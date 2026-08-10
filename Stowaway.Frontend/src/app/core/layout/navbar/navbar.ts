import { Component, computed, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ContainerAdd } from '../container-add/container-add';
import { ItemAdd } from '../item-add/item-add';
import { WarehouseCanvasState } from '../../../services/storage/warehouse-canvas-state';

@Component({
  selector: 'app-navbar',
  imports: [MatButtonModule, MatIconModule, MatTooltipModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar {
  private readonly dialog = inject(MatDialog);
  private readonly canvasState = inject(WarehouseCanvasState);

  readonly warehouse = this.canvasState.warehouse;
  readonly currentContainer = this.canvasState.currentContainer;

  // A container whose type holds 0 sub-containers is the floor of the size
  // hierarchy — nothing can ever nest inside it, so offering "Add container"
  // while standing inside one would only ever end in a rejected request.
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

    const dialogRef = this.dialog.open(ContainerAdd, {
      width: '420px',
      data: {
        warehouseId: warehouse.id,
        parentContainerId: this.currentContainer()?.id ?? null,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.canvasState.notifyLocationChanged();
      }
    });
  }

  openAddItemDialog(): void {
    const warehouse = this.warehouse();
    if (!warehouse) {
      return;
    }

    const dialogRef = this.dialog.open(ItemAdd, {
      width: '420px',
      data: {
        warehouseId: warehouse.id,
        containerId: this.currentContainer()?.id ?? null,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.canvasState.notifyLocationChanged();
      }
    });
  }
}
