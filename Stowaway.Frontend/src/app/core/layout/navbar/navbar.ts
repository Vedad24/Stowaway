import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ContainerAdd } from '../container-add/container-add';
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

  openAddContainerDialog(): void {
    const warehouse = this.warehouse();
    if (!warehouse) {
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
}
