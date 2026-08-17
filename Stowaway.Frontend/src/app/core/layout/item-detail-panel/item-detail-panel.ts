import { Component, computed, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { WarehouseCanvasState } from '../../../services/storage/warehouse-canvas-state';
import { ItemApiService } from '../../../services/storage/item/item';
import { GetItemByIdDto } from '../../../services/storage/item/item.model';
import { ItemEdit } from '../item-edit/item-edit';
import { ConfirmDialog } from '../../../shared/confirm-dialog/confirm-dialog';
import { extractErrorMessage } from '../../../models/http-error';
import { tagColor, tagTextColor } from '../../../shared/tag-color';

@Component({
  selector: 'app-item-detail-panel',
  standalone: true,
  imports: [CommonModule, MatButtonModule],
  templateUrl: './item-detail-panel.html',
  styleUrl: './item-detail-panel.css',
})
export class ItemDetailPanel {
  private readonly canvasState = inject(WarehouseCanvasState);
  private readonly itemService = inject(ItemApiService);
  private readonly dialog = inject(MatDialog);

  private requestToken = 0;

  readonly item = signal<GetItemByIdDto | null>(null);
  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  private readonly selectedItemId = computed(() => {
    const entity = this.canvasState.selectedEntity();
    return entity?.kind === 'item' ? entity.id : null;
  });
  readonly isOpen = computed(() => this.selectedItemId() != null);

  constructor() {
    effect(() => {
      const id = this.selectedItemId();
      if (id == null) {
        this.requestToken++;
        this.item.set(null);
        this.errorMessage.set(null);
        this.isLoading.set(false);
        return;
      }
      this.loadItem(id);
    });
  }

  tagColor(id: number): string {
    return tagColor(id);
  }

  tagTextColor(id: number): string {
    return tagTextColor(tagColor(id));
  }

  getImageSrc(item: GetItemByIdDto): string | null {
    return item.byteImage ? `data:image/jpeg;base64,${item.byteImage}` : null;
  }

  close(): void {
    this.canvasState.clearSelection();
  }

  edit(): void {
    const item = this.item();
    const warehouse = this.canvasState.warehouse();
    if (!item || !warehouse) {
      return;
    }

    const dialogRef = this.dialog.open(ItemEdit, {
      width: '420px',
      data: {
        id: item.id,
        name: item.name,
        description: item.description,
        quantity: item.quantity,
        supplierId: item.supplier.id,
        containerId: item.container.id,
        warehouseId: warehouse.id,
        tagIds: item.tags.map((t) => t.id),
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) {
        return;
      }
      const v = JSON.parse(result);
      this.itemService.update(item.id, {
        name: (v.name ?? '').trim(),
        description: (v.description ?? '').trim(),
        quantity: Number(v.quantity),
        supplierId: Number(v.supplierId),
        containerId: Number(v.containerId),
        tagIds: v.tagIds ? JSON.parse(v.tagIds) : [],
      }).subscribe({
        next: () => {
          this.loadItem(item.id);
          this.canvasState.notifyLocationChanged();
        },
        error: (err) => this.errorMessage.set(extractErrorMessage(err, 'Unable to save changes.')),
      });
    });
  }

  delete(): void {
    const item = this.item();
    if (!item) {
      return;
    }

    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '380px',
      data: {
        title: 'Delete item',
        message: `Are you sure you want to delete "${item.name}"? This cannot be undone.`,
        confirmLabel: 'Delete',
        danger: true,
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) {
        return;
      }
      this.itemService.delete(item.id).subscribe({
        next: () => {
          this.canvasState.clearSelection();
          this.canvasState.notifyLocationChanged();
        },
        error: (err) => this.errorMessage.set(extractErrorMessage(err, 'Unable to delete the item.')),
      });
    });
  }

  private loadItem(id: number): void {
    const token = ++this.requestToken;
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.itemService.getById(id).subscribe({
      next: (item) => {
        if (token !== this.requestToken) {
          return;
        }
        this.item.set(item);
        this.isLoading.set(false);
      },
      error: (err) => {
        if (token !== this.requestToken) {
          return;
        }
        this.errorMessage.set(extractErrorMessage(err, 'Unable to load item details.'));
        this.isLoading.set(false);
      },
    });
  }
}
