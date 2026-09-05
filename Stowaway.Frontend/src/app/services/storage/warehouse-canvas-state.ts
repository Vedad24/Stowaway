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

export interface SelectedEntity {
  kind: 'item' | 'container';
  id: number;
}

export interface LocationChangeScope {
  warehouseId: number;
  // The container(s) whose contents (items or sub-containers) changed, so
  // listeners can refresh just those branches instead of everything they've
  // ever loaded. Null means the warehouse root. A move affects two containers
  // (source and destination) at once — pass both in one call rather than
  // calling notifyLocationChanged twice, since two synchronous signal writes
  // collapse into a single effect run and the first would be lost.
  containerId: number | null | (number | null)[];
}

@Injectable({
  providedIn: 'root',
})
export class WarehouseCanvasState {
  readonly warehouse = signal<CanvasWarehouse | null>(null);
  readonly path = signal<CanvasContainerCrumb[]>([]);
  readonly locationChanged = signal(0);
  readonly lastChangeScope = signal<LocationChangeScope | null>(null);
  readonly selectedEntity = signal<SelectedEntity | null>(null);

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

  // scope is optional so existing call sites keep compiling; omitting it just
  // means listeners fall back to a broader (more expensive) refresh.
  notifyLocationChanged(scope?: LocationChangeScope): void {
    this.lastChangeScope.set(scope ?? null);
    this.locationChanged.update(v => v + 1);
  }

  selectItem(id: number): void {
    this.selectedEntity.set({ kind: 'item', id });
  }

  selectContainer(id: number): void {
    this.selectedEntity.set({ kind: 'container', id });
  }

  clearSelection(): void {
    this.selectedEntity.set(null);
  }
}
