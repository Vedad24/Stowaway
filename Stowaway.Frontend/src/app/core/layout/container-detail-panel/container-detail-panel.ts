import { Component, computed, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { WarehouseCanvasState } from '../../../services/storage/warehouse-canvas-state';
import { ContainerApiService } from '../../../services/storage/container/container';
import { ContainerStatusName, ListContainersQueryDto } from '../../../services/storage/container/container.model';
import { ContainerEdit } from '../container-edit/container-edit';
import { ContainerDelete } from '../container-delete/container-delete';
import { extractErrorMessage } from '../../../models/http-error';
import { tagColor, tagTextColor } from '../../../shared/tag-color';

@Component({
  selector: 'app-container-detail-panel',
  standalone: true,
  imports: [CommonModule, MatButtonModule],
  templateUrl: './container-detail-panel.html',
  styleUrl: './container-detail-panel.css',
})
export class ContainerDetailPanel {
  private readonly canvasState = inject(WarehouseCanvasState);
  private readonly containerService = inject(ContainerApiService);
  private readonly dialog = inject(MatDialog);

  private requestToken = 0;

  readonly container = signal<ListContainersQueryDto | null>(null);
  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly isUpdatingStatus = signal(false);

  private readonly selectedContainerId = computed(() => {
    const entity = this.canvasState.selectedEntity();
    return entity?.kind === 'container' ? entity.id : null;
  });
  readonly isOpen = computed(() => this.selectedContainerId() != null);

  constructor() {
    effect(() => {
      const id = this.selectedContainerId();
      if (id == null) {
        this.requestToken++;
        this.container.set(null);
        this.errorMessage.set(null);
        this.isLoading.set(false);
        return;
      }
      this.loadContainer(id);
    });
  }

  statusColor(id: number): string {
    return tagColor(id);
  }

  statusTextColor(id: number): string {
    return tagTextColor(tagColor(id));
  }

  itemsFullness(container: ListContainersQueryDto): number {
    return container.maxItems > 0 ? Math.min(1, container.itemQuantityUsed / container.maxItems) : 0;
  }

  containersFullness(container: ListContainersQueryDto): number {
    return container.maxContainers > 0 ? Math.min(1, container.containerCountUsed / container.maxContainers) : 0;
  }

  close(): void {
    this.canvasState.clearSelection();
  }

  edit(): void {
    const container = this.container();
    if (!container) {
      return;
    }

    const dialogRef = this.dialog.open(ContainerEdit, {
      width: '420px',
      data: {
        id: container.id,
        name: container.name,
        containerTypeId: container.containerTypeId,
        parentContainerId: container.parentContainerId,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) {
        return;
      }
      const v = JSON.parse(result);
      this.containerService.update(container.id, {
        name: (v.name ?? '').trim(),
        containerTypeId: Number(v.containerTypeId),
      }).subscribe({
        next: () => {
          this.loadContainer(container.id);
          this.canvasState.notifyLocationChanged();
        },
        error: (err) => this.errorMessage.set(extractErrorMessage(err, 'Unable to save changes.')),
      });
    });
  }

  setStatus(status: ContainerStatusName): void {
    const container = this.container();
    if (!container || container.currentStatus?.name === status) {
      return;
    }

    this.isUpdatingStatus.set(true);
    this.containerService.updateStatus(container.id, status).subscribe({
      next: () => {
        this.isUpdatingStatus.set(false);
        this.loadContainer(container.id);
        this.canvasState.notifyLocationChanged();
      },
      error: (err) => {
        this.isUpdatingStatus.set(false);
        this.errorMessage.set(extractErrorMessage(err, 'Unable to update status.'));
      },
    });
  }

  delete(): void {
    const container = this.container();
    if (!container) {
      return;
    }

    const dialogRef = this.dialog.open(ContainerDelete, {
      width: '440px',
      data: {
        id: container.id,
        name: container.name,
        warehouseId: container.warehouseId,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.canvasState.clearSelection();
        this.canvasState.notifyLocationChanged();
      }
    });
  }

  private loadContainer(id: number): void {
    const token = ++this.requestToken;
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.containerService.getById(id).subscribe({
      next: (container) => {
        if (token !== this.requestToken) {
          return;
        }
        this.container.set(container);
        this.isLoading.set(false);
      },
      error: (err) => {
        if (token !== this.requestToken) {
          return;
        }
        this.errorMessage.set(extractErrorMessage(err, 'Unable to load container details.'));
        this.isLoading.set(false);
      },
    });
  }
}
