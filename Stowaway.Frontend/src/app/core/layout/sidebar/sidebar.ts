import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, effect, inject, OnInit, signal, untracked } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { ProductPageService } from '../../../services/sales/product-page/product-page-service';
import { ListWarehousesQueryDto } from '../../../services/sales/product-page/product-page-service.models';
import { ContainerApiService } from '../../../services/storage/container/container';
import { ListContainersQueryDto } from '../../../services/storage/container/container.model';
import { AffectedContainers, WarehouseCanvasState } from '../../../services/storage/warehouse-canvas-state';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { WarehouseReportDialog, WarehouseReportDialogResult } from '../warehouse-report-dialog/warehouse-report-dialog';
import { WarehouseAdd } from '../warehouse-add/warehouse-add';
import { WarehouseApiService } from '../../../services/storage/warehouse/warehouse';
import { ItemApiService } from '../../../services/storage/item/item';
import { ListItemQuery, ListItemQueryDto } from '../../../services/storage/item/item.model';
import { TagApiService } from '../../../services/storage/tag/tag';
import { TagDto } from '../../../services/storage/tag/tag.model';
import { tagColor } from '../../../shared/tag-color';

type ItemTreeNode = ListItemQueryDto & {
  searchMatch?: boolean;
  visible?: boolean;
};

interface ContainerTreeNode extends ListContainersQueryDto {
  expanded: boolean;
  loading: boolean;
  childrenLoaded: boolean;
  children: ContainerTreeNode[];
  items: ItemTreeNode[];
  itemsLoaded: boolean;
  itemsLoading: boolean;
  searchMatch?: boolean;
  visible?: boolean;
  // Bumped on every children-list request for this node; a response is only
  // applied if it's still the most recent request, so an older, slower
  // response can never clobber a newer one that already landed.
  requestToken: number;
  itemsRequestToken: number;
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
  imports: [CommonModule, MatIcon, MatIconModule, MatButtonModule, MatMenuModule, MatDividerModule],
  
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Sidebar implements OnInit {

  private readonly productPageService = inject(ProductPageService);
  private readonly containerService = inject(ContainerApiService);
  private readonly itemService = inject(ItemApiService);
  private readonly canvasState = inject(WarehouseCanvasState);
  private readonly warehouseService = inject(WarehouseApiService);
  private readonly tagService = inject(TagApiService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

  warehouses = signal<WarehouseTreeNode[]>([]);
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  searchText = signal('');
  allTags = signal<TagDto[]>([]);
  selectedTagIds = signal<number[]>([]);
  favouritesOnly = signal(false);

  visibleWarehouses = computed(() => this.warehouses().filter((warehouse) => warehouse.visible !== false));

  constructor() {
    effect(() => {
      const version = this.canvasState.locationChanged();
      if (version === 0) {
        return;
      }
      const affected = this.canvasState.lastAffectedContainers();
      // The refresh below reads and eventually rewrites `warehouses` (a fresh array
      // reference on every response, via refreshTree()). Run it untracked so that
      // rewrite doesn't register as a change to one of THIS effect's own dependencies
      // — otherwise every refresh response would re-trigger this same effect, which
      // would refresh again, which would respond again... an infinite request loop.
      // Only `locationChanged`/`lastAffectedContainers` above should ever re-run this.
      untracked(() => {
        if (affected) {
          this.refreshAffectedContainers(affected);
        } else {
          this.refreshAllLoadedNodes();
        }
      });
    });

    // Separate from the effect above on purpose: a warehouse rename doesn't go
    // through notifyLocationChanged/AffectedContainers (that mechanism only ever
    // re-fetches container lists), so it needs its own signal to patch the one
    // place a warehouse's name is mirrored in this tree — its own row.
    effect(() => {
      const renamed = this.canvasState.warehouseRenamed();
      if (!renamed) {
        return;
      }
      // untracked for the same reason as above: refreshTree() rewrites the
      // `warehouses` signal, and reading `this.warehouses()` in a tracked
      // context here would make this effect its own trigger.
      untracked(() => {
        const warehouse = this.warehouses().find((w) => w.id === renamed.id);
        if (warehouse) {
          warehouse.name = renamed.name;
          this.refreshTree();
        }
      });
    });

    // Same rationale as the rename effect above: a deleted warehouse never
    // touches a container list, so it needs its own signal to know to drop
    // the row from the tree.
    effect(() => {
      const deletedId = this.canvasState.warehouseDeleted();
      if (deletedId == null) {
        return;
      }
      untracked(() => {
        this.warehouses.update((list) => list.filter((w) => w.id !== deletedId));
      });
    });
  }

  ngOnInit(): void {
    this.loadTree();
    this.loadTags();
  }

  updateSearch(value: string): void {
    this.searchText.set(value);
  }

  toggleTagFilter(tagId: number): void {
    const current = this.selectedTagIds();
    this.selectedTagIds.set(
      current.includes(tagId) ? current.filter((id) => id !== tagId) : [...current, tagId],
    );
    this.executeSearch();
  }

  isTagFilterSelected(tagId: number): boolean {
    return this.selectedTagIds().includes(tagId);
  }

  clearFilters(): void {
    this.selectedTagIds.set([]);
    this.favouritesOnly.set(false);
    this.executeSearch();
  }

  hasActiveFilters(): boolean {
    return this.selectedTagIds().length > 0 || this.favouritesOnly();
  }

  activeFilterCount(): number {
    return this.selectedTagIds().length + (this.favouritesOnly() ? 1 : 0);
  }

  toggleFavouritesOnly(): void {
    this.favouritesOnly.update((v) => !v);
    this.executeSearch();
  }

  toggleItemFavourite(item: ItemTreeNode, container: ContainerTreeNode): void {
    const next = !item.isFavourite;
    this.itemService.setFavourite(item.id, next).subscribe({
      next: () => {
        item.isFavourite = next;
        this.refreshTree();
        // Lets the item detail panel (or any other open view of this item) know to
        // refresh, same as every other item mutation in this app signals through.
        this.canvasState.notifyLocationChanged({ warehouseId: container.warehouseId, containerIds: [container.id] });
      },
      error: () => {},
    });
  }

  tagChipColor(id: number): string {
    return tagColor(id);
  }

  executeSearch(): void {
    const textQuery = this.searchText().trim().toLowerCase();
    const tagIds = this.selectedTagIds();
    const favouritesOnly = this.favouritesOnly();

    this.warehouses().forEach((warehouse) => this.clearSearchState(warehouse));

    if (!textQuery && tagIds.length === 0 && !favouritesOnly) {
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
      this.expandMatchingPath(textQuery, tagIds, favouritesOnly, warehouse, null);
    });

    this.refreshTree();
  }

  private loadTags(): void {
    this.tagService.list().subscribe({
      next: (tags) => this.allTags.set(tags),
      error: () => {},
    });
  }

  toggleWarehouse(warehouse: WarehouseTreeNode): void {
    warehouse.expanded = !warehouse.expanded;
    if (warehouse.expanded && !warehouse.childrenLoaded) {
      this.loadChildren(warehouse, warehouse.id, null);
    }
    this.refreshTree();
  }

  toggleContainer(container: ContainerTreeNode): void {
    if (!container.hasChildren && container.itemQuantityUsed <= 0) {
      return;
    }

    container.expanded = !container.expanded;
    if (container.expanded && container.hasChildren && !container.childrenLoaded) {
      this.loadChildren(container, container.warehouseId, container.id);
    }
    if (container.expanded && container.itemQuantityUsed > 0 && !container.itemsLoaded) {
      this.loadItems(container);
    }
    this.refreshTree();
  }

  selectItem(item: ListItemQueryDto): void {
    this.canvasState.selectItem(item.id);
  }

  selectWarehouse(warehouse: WarehouseTreeNode): void {
    this.canvasState.selectWarehouse({ id: warehouse.id, name: warehouse.name });
  }

  viewWarehouseDetails(warehouse: WarehouseTreeNode): void {
    this.canvasState.showWarehouseDetails(warehouse.id);
  }

  addWarehouse(): void {
    const dialogRef = this.dialog.open(WarehouseAdd, {
      width: '420px',
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) {
        return;
      }
      const v = JSON.parse(result);
      this.warehouseService.create({
        name: (v.name ?? '').trim(),
        description: (v.description ?? '').trim(),
        city: (v.city ?? '').trim(),
        address: (v.address ?? '').trim(),
        capacity: Number(v.capacity),
        isEnabled: true,
      }).subscribe({
        next: () => this.loadTree(),
        error: (err) => console.error('Unable to create warehouse.', err),
      });
    });
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

    this.containerService.listAll({
      warehouseId,
      parentContainerId,
    }).subscribe({
      next: (response: ListContainersQueryDto[]) => {
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

  private loadItems(node: ContainerTreeNode, callback?: () => void): void {
    node.itemsLoading = true;
    const requestToken = ++node.itemsRequestToken;
    this.refreshTree();

    const query = new ListItemQuery();
    query.containerId = node.id;

    this.itemService.listAll(query).subscribe({
      next: (response) => {
        if (requestToken !== node.itemsRequestToken) {
          return;
        }
        node.items = response;
        node.itemsLoaded = true;
        node.itemsLoading = false;
        this.refreshTree();
        callback?.();
      },
      error: () => {
        if (requestToken !== node.itemsRequestToken) {
          return;
        }
        node.itemsLoading = false;
        this.errorMessage.set('Unable to load items. Please try again.');
        this.refreshTree();
        callback?.();
      },
    });
  }

  private expandMatchingPath(
    textQuery: string,
    tagIds: number[],
    favouritesOnly: boolean,
    node: WarehouseTreeNode | ContainerTreeNode,
    parent: WarehouseTreeNode | ContainerTreeNode | null,
  ): boolean {
    // Tags only apply to items, not containers/warehouses, so a container can
    // only self-match on its name; with no text query it never self-matches
    // and only becomes visible by containing a matching item below.
    const matches = textQuery ? (node.name ?? '').toLowerCase().includes(textQuery) : false;
    let hasMatchInChildren = false;
    let hasMatchInItems = false;
    const isContainer = this.isContainerNode(node);

    if (!node.childrenLoaded && !node.loading) {
      if (!isContainer) {
        this.loadChildren(node, node.id, null, () => {
          this.expandMatchingPath(textQuery, tagIds, favouritesOnly, node, parent);
          this.refreshTree();
        });
        return matches;
      }

      if (node.hasChildren) {
        this.loadChildren(node, node.warehouseId, node.id, () => {
          this.expandMatchingPath(textQuery, tagIds, favouritesOnly, node, parent);
          this.refreshTree();
        });
        return matches;
      }
    }

    if (isContainer && node.itemQuantityUsed > 0 && !node.itemsLoaded && !node.itemsLoading) {
      this.loadItems(node, () => {
        this.expandMatchingPath(textQuery, tagIds, favouritesOnly, node, parent);
        this.refreshTree();
      });
      return matches;
    }

    if (node.childrenLoaded) {
      node.children.forEach((child) => {
        if (this.expandMatchingPath(textQuery, tagIds, favouritesOnly, child, node)) {
          hasMatchInChildren = true;
        }
      });
    }

    if (isContainer && node.itemsLoaded) {
      node.items.forEach((item) => {
        const nameMatches = !textQuery || (item.name ?? '').toLowerCase().includes(textQuery);
        const tagMatches = tagIds.length === 0 || (item.tags ?? []).some((tag) => tagIds.includes(tag.id));
        const favouriteMatches = !favouritesOnly || item.isFavourite;
        const itemMatches = nameMatches && tagMatches && favouriteMatches;
        item.visible = itemMatches;
        item.searchMatch = itemMatches;
        if (itemMatches) {
          hasMatchInItems = true;
        }
      });
    }

    const shouldShow = matches || hasMatchInChildren || hasMatchInItems;

    if (shouldShow) {
      node.visible = true;
      node.searchMatch = matches;
      node.expanded = (hasMatchInChildren || hasMatchInItems) && !matches;

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
    if (this.isContainerNode(node)) {
      node.items.forEach((item) => {
        item.searchMatch = false;
        item.visible = false;
      });
    }
  }

  private resetVisibility(node: WarehouseTreeNode | ContainerTreeNode): void {
    node.visible = true;
    node.children.forEach((child) => this.resetVisibility(child));
    if (this.isContainerNode(node)) {
      node.items.forEach((item) => {
        item.visible = true;
      });
    }
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
      items: [],
      itemsLoaded: false,
      itemsLoading: false,
      requestToken: 0,
      itemsRequestToken: 0,
    };
  }

  private refreshTree(): void {
    this.warehouses.update(warehouses => [...warehouses]);
  }

  // Refreshes only the branches an add/edit/delete could actually have touched:
  // each affected container's own children/items (if loaded — the change may have
  // happened directly inside it) plus its parent's children (if loaded — the
  // container's own row, e.g. its item/child count, may need updating there).
  // The parent is found by searching the tree we already have loaded, so callers
  // only ever need to name the container(s) that gained or lost something directly.
  private refreshAffectedContainers(affected: AffectedContainers): void {
    const uniqueIds = Array.from(new Set(affected.containerIds));
    uniqueIds.forEach((containerId) => this.refreshContainerAndItsRow(affected.warehouseId, containerId));
  }

  private refreshContainerAndItsRow(warehouseId: number, containerId: number | null): void {
    const warehouse = this.warehouses().find((w) => w.id === warehouseId);
    if (!warehouse) {
      return;
    }

    if (containerId == null) {
      if (warehouse.childrenLoaded) {
        this.refreshNodeChildren(warehouse, warehouseId, null);
      }
      return;
    }

    const located = this.findContainerNode(warehouse, containerId);
    if (!located) {
      return;
    }
    const { node, parent } = located;

    if (node.childrenLoaded) {
      this.refreshNodeChildren(node, warehouseId, node.id);
    }
    if (node.itemsLoaded) {
      this.refreshNodeItems(node);
    }
    if (parent.childrenLoaded) {
      const parentContainerId = this.isContainerNode(parent) ? parent.id : null;
      this.refreshNodeChildren(parent, warehouseId, parentContainerId);
    }
  }

  private findContainerNode(
    warehouse: WarehouseTreeNode,
    containerId: number,
  ): { node: ContainerTreeNode; parent: WarehouseTreeNode | ContainerTreeNode } | null {
    const search = (
      parent: WarehouseTreeNode | ContainerTreeNode,
    ): { node: ContainerTreeNode; parent: WarehouseTreeNode | ContainerTreeNode } | null => {
      for (const child of parent.children) {
        if (child.id === containerId) {
          return { node: child, parent };
        }
        const found = search(child);
        if (found) {
          return found;
        }
      }
      return null;
    };
    return search(warehouse);
  }

  // Fallback for callers that can't name the affected container(s): re-fetch every
  // branch of the tree that's currently expanded. The whole tree is walked up front
  // (synchronously, off the state as it stands right now) so every branch's request
  // fires at once instead of cascading one level at a time — a tree several levels
  // deep no longer takes several sequential round-trips to catch up.
  private refreshAllLoadedNodes(): void {
    const containerTargets: { node: WarehouseTreeNode | ContainerTreeNode; warehouseId: number; parentContainerId: number | null }[] = [];
    const itemTargets: ContainerTreeNode[] = [];

    const collect = (node: WarehouseTreeNode | ContainerTreeNode, warehouseId: number, parentContainerId: number | null): void => {
      if (this.isContainerNode(node) && node.itemsLoaded) {
        itemTargets.push(node);
      }
      if (!node.childrenLoaded) {
        return;
      }
      containerTargets.push({ node, warehouseId, parentContainerId });
      node.children.forEach((child) => collect(child, warehouseId, child.id));
    };

    this.warehouses().forEach((warehouse) => collect(warehouse, warehouse.id, null));
    containerTargets.forEach(({ node, warehouseId, parentContainerId }) => this.refreshNodeChildren(node, warehouseId, parentContainerId));
    itemTargets.forEach((node) => this.refreshNodeItems(node));
  }

  private refreshNodeChildren(
    node: WarehouseTreeNode | ContainerTreeNode,
    warehouseId: number,
    parentContainerId: number | null,
  ): void {
    const requestToken = ++node.requestToken;

    this.containerService.listAll({ warehouseId, parentContainerId }).subscribe({
      next: (response: ListContainersQueryDto[]) => {
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

  private refreshNodeItems(node: ContainerTreeNode): void {
    const requestToken = ++node.itemsRequestToken;

    const query = new ListItemQuery();
    query.containerId = node.id;

    this.itemService.listAll(query).subscribe({
      next: (response) => {
        if (requestToken !== node.itemsRequestToken) {
          return;
        }
        node.items = response;
        this.refreshTree();
      },
      error: () => {},
    });
  }


  goToPriviledges(warehouseId : number) {
    this.router.navigate(['priviledge-group/edit', warehouseId]);
  }

  openReportDialog(warehouse: WarehouseTreeNode): void {
    const dialogRef = this.dialog.open(WarehouseReportDialog, {
      width: '380px',
      data: { warehouseName: warehouse.name },
    });

    dialogRef.afterClosed().subscribe((result: WarehouseReportDialogResult | undefined) => {
      if (!result) {
        return;
      }
      this.router.navigate(['/report', warehouse.id], {
        queryParams: {
          type: result.type,
          containerColumns: result.containerColumns,
          itemColumns: result.itemColumns,
        },
      });
    });
  }
}

