import { Component, computed, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { ContainerAdd } from '../container-add/container-add';
import { ItemAdd } from '../item-add/item-add';
import { WarehouseCanvasState } from '../../../services/storage/warehouse-canvas-state';
import { ItemApiService } from '../../../services/storage/item/item';
import { ContainerApiService } from '../../../services/storage/container/container';
import { ThemeToggle } from '../../../shared/theme-toggle/theme-toggle';

@Component({
  selector: 'app-navbar',
  imports: [MatButtonModule, MatIconModule, MatTooltipModule, RouterLink, ThemeToggle],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar {
  private readonly dialog = inject(MatDialog);
  private readonly canvasState = inject(WarehouseCanvasState);
  private readonly itemService = inject(ItemApiService);
  private readonly containerService = inject(ContainerApiService);

  readonly warehouse = this.canvasState.warehouse;
  readonly currentContainer = this.canvasState.currentContainer;
  
  readonly canAddContainer = computed(() => {
    if (!this.warehouse()) {
      return false;
    }
    const current = this.currentContainer();
    return current == null || current.maxContainers > 0;
  });

  readonly addContainerTooltip = computed(() => {
    if (!this.warehouse()) {
      return 'Select a warehouse first';
    }
    const current = this.currentContainer();
    if (current && current.maxContainers === 0) {
      return "This container can't hold sub-containers";
    }
    return '';
  });

  openAddContainerDialog(): void {
    const warehouse = this.warehouse();
    if (!warehouse || !this.canAddContainer()) {
      return;
    }

    const parentContainerId = this.currentContainer()?.id ?? null;

    const dialogRef = this.dialog.open(ContainerAdd, {
      width: '420px',
      data: {
        warehouseId: warehouse.id,
        parentContainerId,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) {
        return;
      }
      const v = JSON.parse(result);
      this.containerService.create({
        name: (v.name ?? '').trim(),
        containerTypeId: Number(v.containerTypeId),
        warehouseId: warehouse.id,
        parentContainerId,
      }).subscribe({
        next: () => this.canvasState.notifyLocationChanged({ warehouseId: warehouse.id, containerId: parentContainerId }),
        error: (err) => console.error('Unable to create container.', err),
      });
    });
  }

  openAddItemDialog(): void {
    const warehouse = this.warehouse();
    if (!warehouse) {
      return;
    }

    const dialogRef = this.dialog.open(ItemAdd, {
      width: '840px',
      maxWidth: '95vw',
      data: {
        warehouseId: warehouse.id,
        containerId: this.currentContainer()?.id ?? null,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) {
        return;
      }
      const v = JSON.parse(result);
      this.itemService.create({
        name: (v.name ?? '').trim(),
        description: (v.description ?? '').trim(),
        quantity: Number(v.quantity),
        supplierId: Number(v.supplierId),
        containerId: Number(v.containerId),
        tagIds: v.tagIds ? JSON.parse(v.tagIds) : [],
        images: v.images ? JSON.parse(v.images) : [],
      }).subscribe({
        next: () => this.canvasState.notifyLocationChanged({ warehouseId: warehouse.id, containerId: Number(v.containerId) }),
        error: (err) => console.error('Unable to create item.', err),
      });
    });
  }
}
