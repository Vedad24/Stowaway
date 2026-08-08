import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, effect, inject, OnInit, signal, untracked } from '@angular/core';
import { Router } from '@angular/router';
import { ProductPageService } from '../../../services/sales/product-page/product-page-service';
import { ListWarehousesQueryDto } from '../../../services/sales/product-page/product-page-service.models';
import { ContainerApiService } from '../../../services/storage/container/container';
import { ListContainersQueryResponse, ListContainersQueryDto } from '../../../services/storage/container/container.model';
import { WarehouseCanvasState } from '../../../services/storage/warehouse-canvas-state';
import { MatIcon, MatIconModule } from '@angular/material/icon';

interface ContainerTreeNode extends ListContainersQueryDto {
  expanded: boolean;
  loading: boolean;
  childrenLoaded: boolean;
  children: ContainerTreeNode[];
  searchMatch?: boolean;
  visible?: boolean;
  // Bumped on every children-list request for this node; a response is only
  // applied if it's still the most recent request, so an older, slower
  // response can never clobber a newer one that already landed.
  requestToken: number;
}

interface WarehouseTreeNode extends ListWarehousesQueryDto {
  expanded: boolean;
  loading: boolean;
  childrenLoaded: boolean;
  children: ContainerTreeNode[];
  searchMatch?: boolean;
  visible?: boolean;
  requestToken: number;
}

@Component({
  selector: 'app-sidebar',
  imports: [CommonModule, MatIcon, MatIconModule],
  
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Sidebar implements OnInit {

  private readonly productPageService = inject(ProductPageService);
  private readonly containerService = inject(ContainerApiService);
  private readonly canvasState = inject(WarehouseCanvasState);
  private readonly router = inject(Router);

  warehouses = signal<WarehouseTreeNode[]>([]);
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  searchText = signal('');

  visibleWarehouses = computed(() => this.warehouses().filter((warehouse) => warehouse.visible !== false));

  constructor() {
    effect(() => {
      const version = this.canvasState.locationChanged();
      if (version === 0) {
        return;
      }
      // refreshLoadedNodes() reads the `warehouses` signal (and rewrites it once
      // responses land). Without untracked(), that read makes `warehouses` a
      // second dependency of this effect — its own write-back then re-triggers
      // this same effect, which re-reads it and writes again, forever.
      untracked(() => this.refreshLoadedNodes());
    });
  }

  ngOnInit(): void {
    this.loadTree();
  }

  updateSearch(value: string): void {
    this.searchText.set(value);
  }

  executeSearch(): void {
    const query = this.searchText().trim().toLowerCase();

    this.warehouses().forEach((warehouse) => this.clearSearchState(warehouse));

    if (!query) {
      this.warehouses().forEach((warehouse) => {
        warehouse.visible = true;
        warehouse.expanded = false;
        warehouse.children.forEach((child) => this.resetVisibility(child));
      });
      this.refreshTree();
      return;
    }

    this.warehouses().forEach((warehouse) => {
      warehouse.visible = false;
      this.expandMatchingPath(query, warehouse, null);
    });

    this.refreshTree();
  }

  toggleWarehouse(warehouse: WarehouseTreeNode): void {
    warehouse.expanded = !warehouse.expanded;
    if (warehouse.expanded && !warehouse.childrenLoaded) {
      this.loadChildren(warehouse, warehouse.id, null);
    }
    this.refreshTree();
  }

  toggleContainer(container: ContainerTreeNode): void {
    if (!container.hasChildren) {
      return;
    }

    container.expanded = !container.expanded;
    if (container.expanded && !container.childrenLoaded) {
      this.loadChildren(container, container.warehouseId, container.id);
    }
    this.refreshTree();
  }

  selectWarehouse(warehouse: WarehouseTreeNode): void {
    this.canvasState.selectWarehouse({ id: warehouse.id, name: warehouse.name });
  }

  withAncestor(ancestors: ContainerTreeNode[], container: ContainerTreeNode): ContainerTreeNode[] {
    return [...ancestors, container];
  }

  selectContainer(container: ContainerTreeNode, ancestors: ContainerTreeNode[], warehouse: WarehouseTreeNode): void {
    this.canvasState.navigateTo(
      { id: warehouse.id, name: warehouse.name },
      [...ancestors, container].map(node => ({
        id: node.id,
        name: node.name,
        maxItems: node.maxItems,
        maxContainers: node.maxContainers,
      })),
    );
  }

  private loadTree(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.productPageService.getUserWarehouses().subscribe({
      next: (response) => {
        this.warehouses.set((response.items ?? []).map(warehouse => ({
          ...warehouse,
          expanded: false,
          loading: false,
          childrenLoaded: false,
          children: [],
          requestToken: 0,
        })));
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set('Unable to load your warehouses.');
        this.isLoading.set(false);
      },
    });
  }

  private loadChildren(
    node: WarehouseTreeNode | ContainerTreeNode,
    warehouseId: number,
    parentContainerId: number | null,
    callback?: () => void,
  ): void {
    node.loading = true;
    const requestToken = ++node.requestToken;
    this.refreshTree();

    this.containerService.list({
      warehouseId,
      parentContainerId,
    }).subscribe({
      next: (response: ListContainersQueryResponse) => {
        if (requestToken !== node.requestToken) {
          return;
        }
        node.children = response.map(container => this.toTreeNode(container));
        node.childrenLoaded = true;
        node.loading = false;
        this.refreshTree();
        callback?.();
      },
      error: () => {
        if (requestToken !== node.requestToken) {
          return;
        }
        node.loading = false;
        this.errorMessage.set('Unable to load containers. Please try again.');
        this.refreshTree();
        callback?.();
      },
    });
  }

  private expandMatchingPath(
    query: string,
    node: WarehouseTreeNode | ContainerTreeNode,
    parent: WarehouseTreeNode | ContainerTreeNode | null,
  ): boolean {
    const matches = (node.name ?? '').toLowerCase().includes(query);
    let hasMatchInChildren = false;

    if (!node.childrenLoaded && !node.loading) {
      if (!this.isContainerNode(node)) {
        this.loadChildren(node, node.id, null, () => {
          this.expandMatchingPath(query, node, parent);
          this.refreshTree();
        });
        return matches;
      }

      if (node.hasChildren) {
        this.loadChildren(node, node.warehouseId, node.id, () => {
          this.expandMatchingPath(query, node, parent);
          this.refreshTree();
        });
        return matches;
      }
    }

    if (node.childrenLoaded) {
      node.children.forEach((child) => {
        if (this.expandMatchingPath(query, child, node)) {
          hasMatchInChildren = true;
        }
      });
    }

    const shouldShow = matches || hasMatchInChildren;

    if (shouldShow) {
      node.visible = true;
      node.searchMatch = matches;
      node.expanded = hasMatchInChildren && !matches;

      if (parent) {
        parent.visible = true;
        parent.expanded = true;
      }

      return true;
    }

    node.visible = false;
    node.searchMatch = false;
    node.expanded = false;
    return false;
  }

  private clearSearchState(node: WarehouseTreeNode | ContainerTreeNode): void {
    node.searchMatch = false;
    node.visible = false;
    node.expanded = false;
    node.children.forEach((child) => this.clearSearchState(child));
  }

  private resetVisibility(node: WarehouseTreeNode | ContainerTreeNode): void {
    node.visible = true;
    node.children.forEach((child) => this.resetVisibility(child));
  }

  private isContainerNode(node: WarehouseTreeNode | ContainerTreeNode): node is ContainerTreeNode {
    return 'warehouseId' in node;
  }

  private toTreeNode(container: ListContainersQueryDto): ContainerTreeNode {
    return {
      ...container,
      expanded: false,
      loading: false,
      childrenLoaded: false,
      children: [],
      requestToken: 0,
    };
  }

  private refreshTree(): void {
    this.warehouses.update(warehouses => [...warehouses]);
  }

  // After a move, re-fetch every branch of the tree that's currently expanded.
  // The whole tree is walked up front (synchronously, off the state as it stands
  // right now) so every branch's request fires at once instead of cascading one
  // level at a time — a tree several levels deep no longer takes several
  // sequential round-trips to catch up.
  private refreshLoadedNodes(): void {
    const targets: { node: WarehouseTreeNode | ContainerTreeNode; warehouseId: number; parentContainerId: number | null }[] = [];

    const collect = (node: WarehouseTreeNode | ContainerTreeNode, warehouseId: number, parentContainerId: number | null): void => {
      if (!node.childrenLoaded) {
        return;
      }
      targets.push({ node, warehouseId, parentContainerId });
      node.children.forEach((child) => collect(child, warehouseId, child.id));
    };

    this.warehouses().forEach((warehouse) => collect(warehouse, warehouse.id, null));
    targets.forEach(({ node, warehouseId, parentContainerId }) => this.refreshNodeChildren(node, warehouseId, parentContainerId));
  }

  private refreshNodeChildren(
    node: WarehouseTreeNode | ContainerTreeNode,
    warehouseId: number,
    parentContainerId: number | null,
  ): void {
    const requestToken = ++node.requestToken;

    this.containerService.list({ warehouseId, parentContainerId }).subscribe({
      next: (response: ListContainersQueryResponse) => {
        if (requestToken !== node.requestToken) {
          return;
        }
        const previousById = new Map(node.children.map((child) => [child.id, child]));
        // Mutate matched nodes in place rather than replacing them: a toggleContainer()
        // expand may still be in flight for one of these, and its callback closes over
        // the existing object reference. Swapping in a fresh copy here would orphan that
        // callback, leaving the node's `loading` flag stuck true forever.
        node.children = response.map((container) => {
          const previous = previousById.get(container.id);
          if (previous) {
            Object.assign(previous, container);
            return previous;
          }
          return this.toTreeNode(container);
        });
        this.refreshTree();
      },
      error: () => {},
    });
  }

  
  goToPriviledges(warehouseId : number) {
    this.router.navigate(['priviledge-group/edit', warehouseId]);
  }
}

