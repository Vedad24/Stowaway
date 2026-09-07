import { Component, computed, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { WarehouseCanvasState } from '../../../services/storage/warehouse-canvas-state';
import { WarehouseApiService } from '../../../services/storage/warehouse/warehouse';
import { GetWarehouseByIdDto } from '../../../services/storage/warehouse/warehouse.model';
import { WarehouseEdit } from '../warehouse-edit/warehouse-edit';
import { ConfirmDialog } from '../../../shared/confirm-dialog/confirm-dialog';
import { extractErrorMessage } from '../../../models/http-error';
import { CurrentUserService } from '../../../services/identity/auth/current-user-service';
import { Permissions } from '../../../shared/constants/permissions';

@Component({
  selector: 'app-warehouse-detail-panel',
  standalone: true,
  imports: [CommonModule, MatButtonModule],
  templateUrl: './warehouse-detail-panel.html',
  styleUrl: './warehouse-detail-panel.css',
})
export class WarehouseDetailPanel {
  private readonly canvasState = inject(WarehouseCanvasState);
  private readonly warehouseService = inject(WarehouseApiService);
  private readonly dialog = inject(MatDialog);
  private readonly currentUser = inject(CurrentUserService);

  private requestToken = 0;

  readonly warehouse = signal<GetWarehouseByIdDto | null>(null);
  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly canDelete = computed(() =>
    this.currentUser.isAdmin || this.currentUser.permissions.includes(Permissions.WarehouseDelete),
  );

  private readonly selectedWarehouseId = computed(() => {
    const entity = this.canvasState.selectedEntity();
    return entity?.kind === 'warehouse' ? entity.id : null;
  });
  readonly isOpen = computed(() => this.selectedWarehouseId() != null);

  constructor() {
    effect(() => {
      const id = this.selectedWarehouseId();
      if (id == null) {
        this.requestToken++;
        this.warehouse.set(null);
        this.errorMessage.set(null);
        this.isLoading.set(false);
        return;
      }
      this.loadWarehouse(id);
    });
  }

  close(): void {
    this.canvasState.clearSelection();
  }

  edit(): void {
    const warehouse = this.warehouse();
    if (!warehouse) {
      return;
    }

    const dialogRef = this.dialog.open(WarehouseEdit, {
      width: '420px',
      data: {
        id: warehouse.id,
        name: warehouse.name,
        description: warehouse.description,
        city: warehouse.city,
        address: warehouse.address,
        capacity: warehouse.capacity,
        isEnabled: warehouse.isEnabled,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) {
        return;
      }
      const v = JSON.parse(result);
      const name = (v.name ?? '').trim();
      this.warehouseService.update(warehouse.id, {
        name,
        description: (v.description ?? '').trim(),
        city: (v.city ?? '').trim(),
        address: (v.address ?? '').trim(),
        capacity: Number(v.capacity),
        isEnabled: warehouse.isEnabled,
      }).subscribe({
        next: () => {
          this.loadWarehouse(warehouse.id);
          this.canvasState.notifyWarehouseRenamed(warehouse.id, name);
        },
        error: (err) => this.errorMessage.set(extractErrorMessage(err, 'Unable to save changes.')),
      });
    });
  }

  delete(): void {
    const warehouse = this.warehouse();
    if (!warehouse || !this.canDelete()) {
      return;
    }

    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '380px',
      data: {
        title: 'Delete warehouse',
        message: `Are you sure you want to delete "${warehouse.name}"? This cannot be undone.`,
        confirmLabel: 'Delete',
        danger: true,
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) {
        return;
      }
      this.warehouseService.delete(warehouse.id).subscribe({
        next: () => {
          this.canvasState.clearSelection();
          this.canvasState.notifyWarehouseDeleted(warehouse.id);
        },
        error: (err) => this.errorMessage.set(extractErrorMessage(err, 'Unable to delete the warehouse.')),
      });
    });
  }

  private loadWarehouse(id: number): void {
    const token = ++this.requestToken;
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.warehouseService.getById(id).subscribe({
      next: (warehouse) => {
        if (token !== this.requestToken) {
          return;
        }
        this.warehouse.set(warehouse);
        this.isLoading.set(false);
      },
      error: (err) => {
        if (token !== this.requestToken) {
          return;
        }
        this.errorMessage.set(extractErrorMessage(err, 'Unable to load warehouse details.'));
        this.isLoading.set(false);
      },
    });
  }
}
