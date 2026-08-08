import { Injectable, computed, signal } from '@angular/core';

export interface CanvasWarehouse {
  id: number;
  name: string;
}

export interface CanvasContainerCrumb {
  id: number;
  name: string;
  maxItems: number;
  maxContainers: number;
}

@Injectable({
  providedIn: 'root',
})
export class WarehouseCanvasState {
  readonly warehouse = signal<CanvasWarehouse | null>(null);
  readonly path = signal<CanvasContainerCrumb[]>([]);
  readonly locationChanged = signal(0);

  readonly currentContainer = computed<CanvasContainerCrumb | null>(() => {
    const path = this.path();
    return path.length ? path[path.length - 1] : null;
  });

  selectWarehouse(warehouse: CanvasWarehouse): void {
    this.warehouse.set(warehouse);
    this.path.set([]);
  }

  navigateTo(warehouse: CanvasWarehouse, containers: CanvasContainerCrumb[]): void {
    this.warehouse.set(warehouse);
    this.path.set(containers);
  }

  enterContainer(container: CanvasContainerCrumb): void {
    this.path.update(path => [...path, container]);
  }

  goToRoot(): void {
    this.path.set([]);
  }

  goToCrumb(index: number): void {
    this.path.update(path => path.slice(0, index + 1));
  }

  notifyLocationChanged(): void {
    this.locationChanged.update(v => v + 1);
  }
}
