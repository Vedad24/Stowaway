import { Component, computed, effect, inject, signal, untracked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { WarehouseCanvasState } from '../../../services/storage/warehouse-canvas-state';
import { ItemApiService } from '../../../services/storage/item/item';
import { GetItemByIdDto } from '../../../services/storage/item/item.model';
import { ItemEdit } from '../item-edit/item-edit';
import { ConfirmDialog } from '../../../shared/confirm-dialog/confirm-dialog';
import { ImageLightbox } from '../../../shared/image-lightbox/image-lightbox';
import { extractErrorMessage } from '../../../models/http-error';
import { tagColor, tagTextColor } from '../../../shared/tag-color';

@Component({
  selector: 'app-item-detail-panel',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule],
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

    // Other places that can change this item's data (the sidebar's own favourite
    // toggle, edits/moves elsewhere) all signal through notifyLocationChanged — so
    // re-fetch here too, otherwise this panel can go stale while still open.
    effect(() => {
      const version = this.canvasState.locationChanged();
      if (version === 0) {
        return;
      }
      untracked(() => {
        const id = this.selectedItemId();
        if (id != null) {
          this.loadItem(id);
        }
      });
    });
  }

  tagColor(id: number): string {
    return tagColor(id);
  }

  tagTextColor(id: number): string {
    return tagTextColor(tagColor(id));
  }

  readonly carouselIndex = signal(0);

  readonly imageSrcs = computed<string[]>(() => {
    const data = this.item();
    if (!data) {
      return [];
    }
    if (data.images?.length) {
      return data.images
        .slice()
        .sort((a, b) => a.sortOrder - b.sortOrder)
        .map((i) => `data:image/jpeg;base64,${i.byteImage}`);
    }
    if (data.byteImage) {
      return [`data:image/jpeg;base64,${data.byteImage}`];
    }
    return [];
  });

  nextImage(): void {
    const n = this.imageSrcs().length;
    if (n) {
      this.carouselIndex.set((this.carouselIndex() + 1) % n);
    }
  }

  prevImage(): void {
    const n = this.imageSrcs().length;
    if (n) {
      this.carouselIndex.set((this.carouselIndex() - 1 + n) % n);
    }
  }

  goToImage(index: number): void {
    this.carouselIndex.set(index);
  }

  openLightbox(): void {
    const images = this.imageSrcs();
    if (!images.length) {
      return;
    }
    this.dialog.open(ImageLightbox, {
      width: '90vw',
      height: '85vh',
      maxWidth: '1200px',
      panelClass: 'image-lightbox-panel',
      data: { images, startIndex: this.carouselIndex() },
    });
  }

  close(): void {
    this.canvasState.clearSelection();
  }

  toggleFavourite(): void {
    const data = this.item();
    if (!data) {
      return;
    }
    const next = !data.isFavourite;
    this.itemService.setFavourite(data.id, next).subscribe({
      next: () => {
        this.item.update((current) => (current ? { ...current, isFavourite: next } : current));
        // This panel can be opened straight from an item row without the warehouse/path
        // signals ever being set (those only get set by clicking a warehouse or container
        // row), so there's no reliable warehouseId to scope the refresh to — omitting
        // `affected` falls back to refreshing every loaded branch instead.
        this.canvasState.notifyLocationChanged();
      },
      error: (err) => this.errorMessage.set(extractErrorMessage(err, 'Unable to update favourite.')),
    });
  }

  edit(): void {
    const item = this.item();
    const warehouse = this.canvasState.warehouse();
    if (!item || !warehouse) {
      return;
    }

    const dialogRef = this.dialog.open(ItemEdit, {
      width: '840px',
      maxWidth: '95vw',
      data: {
        id: item.id,
        name: item.name,
        description: item.description,
        quantity: item.quantity,
        supplierId: item.supplier.id,
        containerId: item.container.id,
        warehouseId: warehouse.id,
        tagIds: item.tags.map((t) => t.id),
        images: item.images,
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
        images: v.images ? JSON.parse(v.images) : [],
      }).subscribe({
        next: () => {
          this.loadItem(item.id);
          this.canvasState.notifyLocationChanged({
            warehouseId: warehouse.id,
            containerIds: [item.container.id, Number(v.containerId)],
          });
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
      const warehouse = this.canvasState.warehouse();
      this.itemService.delete(item.id).subscribe({
        next: () => {
          this.canvasState.clearSelection();
          this.canvasState.notifyLocationChanged(
            warehouse ? { warehouseId: warehouse.id, containerIds: [item.container.id] } : undefined,
          );
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
        this.carouselIndex.set(0);
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
