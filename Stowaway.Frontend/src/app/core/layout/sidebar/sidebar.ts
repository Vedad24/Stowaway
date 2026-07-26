import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, OnInit, signal } from '@angular/core';
import { ProductPageService } from '../../../services/sales/product-page/product-page-service';
import { ListWarehousesQueryDto } from '../../../services/sales/product-page/product-page-service.models';
import { ContainerApiService } from '../../../services/storage/container/container';
import { ListContainersQueryResponse, ListContainersQueryDto } from '../../../services/storage/container/container.model';

interface ContainerTreeNode extends ListContainersQueryDto {
  expanded: boolean;
  loading: boolean;
  childrenLoaded: boolean;
  children: ContainerTreeNode[];
  searchMatch?: boolean;
  visible?: boolean;
}

interface WarehouseTreeNode extends ListWarehousesQueryDto {
  expanded: boolean;
  loading: boolean;
  childrenLoaded: boolean;
  children: ContainerTreeNode[];
  searchMatch?: boolean;
  visible?: boolean;
}

@Component({
  selector: 'app-sidebar',
  imports: [CommonModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Sidebar implements OnInit {
  private readonly productPageService = inject(ProductPageService);
  private readonly containerService = inject(ContainerApiService);

  warehouses = signal<WarehouseTreeNode[]>([]);
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  searchText = signal('');

  visibleWarehouses = computed(() => this.warehouses().filter((warehouse) => warehouse.visible !== false));

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
    this.refreshTree();

    this.containerService.list({
      warehouseId,
      parentContainerId,
    }).subscribe({
      next: (response: ListContainersQueryResponse) => {
        node.children = response.map(container => this.toTreeNode(container));
        node.childrenLoaded = true;
        node.loading = false;
        this.refreshTree();
        callback?.();
      },
      error: () => {
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
    };
  }

  private refreshTree(): void {
    this.warehouses.update(warehouses => [...warehouses]);
  }
}
