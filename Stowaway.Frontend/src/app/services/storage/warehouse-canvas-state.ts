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
  kind: 'item' | 'container' | 'warehouse';
  id: number;
}

// Reported alongside notifyLocationChanged() so listeners can refresh just the
// branches that could actually be stale, instead of everything they have loaded.
// `containerIds` holds every container whose direct contents changed (`null`
// stands for the warehouse root) — e.g. both the old and new container for a move.
export interface AffectedContainers {
  warehouseId: number;
  containerIds: (number | null)[];
}

@Injectable({
  providedIn: 'root',
})
export class WarehouseCanvasState {
  readonly warehouse = signal<CanvasWarehouse | null>(null);
  readonly path = signal<CanvasContainerCrumb[]>([]);
  readonly locationChanged = signal(0);
  readonly lastAffectedContainers = signal<AffectedContainers | null>(null);
  readonly selectedEntity = signal<SelectedEntity | null>(null);
  readonly warehouseRenamed = signal<{ id: number; name: string } | null>(null);

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

  // `affected`, when given, scopes what listeners need to refresh. Omit it only
  // when the affected container(s) genuinely can't be pinned down, so listeners
  // fall back to refreshing everything they have loaded.
  notifyLocationChanged(affected?: AffectedContainers): void {
    this.lastAffectedContainers.set(affected ?? null);
    this.locationChanged.update(v => v + 1);
  }

  selectItem(id: number): void {
    this.selectedEntity.set({ kind: 'item', id });
  }

  selectContainer(id: number): void {
    this.selectedEntity.set({ kind: 'container', id });
  }

  showWarehouseDetails(id: number): void {
    this.selectedEntity.set({ kind: 'warehouse', id });
  }

  clearSelection(): void {
    this.selectedEntity.set(null);
  }

  // Separate from notifyLocationChanged/AffectedContainers on purpose: that
  // mechanism only ever re-fetches container lists. A warehouse rename doesn't
  // touch any container, but the sidebar tree still needs to know to patch the
  // one place a warehouse's name is mirrored — its own tree row.
  notifyWarehouseRenamed(id: number, name: string): void {
    this.warehouseRenamed.set({ id, name });
  }
}
