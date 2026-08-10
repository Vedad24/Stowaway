import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, signal } from '@angular/core';
import { CdkDrag, CdkDragEnd, CdkDragMove, DragDropModule } from '@angular/cdk/drag-drop';
import { MatDialog } from '@angular/material/dialog';
import { WarehouseCanvasState } from '../../../services/storage/warehouse-canvas-state';
import { ContainerApiService } from '../../../services/storage/container/container';
import { ListContainersQueryDto } from '../../../services/storage/container/container.model';
import { ItemApiService } from '../../../services/storage/item/item';
import { ListItemQuery, ListItemQueryDto } from '../../../services/storage/item/item.model';
import { ContainerEdit } from '../container-edit/container-edit';
import { ContainerDelete } from '../container-delete/container-delete';
import { ItemEdit } from '../item-edit/item-edit';
import { ConfirmDialog } from '../../../shared/confirm-dialog/confirm-dialog';
import { extractErrorMessage } from '../../../models/http-error';

interface Position {
  x: number;
  y: number;
}

type EntityKind = 'container' | 'item';

// Where a drag ends up: nested into another container, dropped at a spot
// on the canvas, or dropped outside the canvas entirely (which unplaces it).
type DropResolution =
  | { action: 'nest'; containerId: number }
  | { action: 'position'; position: Position }
  | { action: 'outside' };

// A card is roughly 112x48 — these offsets center it under the pointer on drop.
const CARD_DROP_OFFSET_X = 56;
const CARD_DROP_OFFSET_Y = 24;

@Component({
  selector: 'app-warehouse-canvas',
  imports: [CommonModule, DragDropModule],
  templateUrl: './warehouse-canvas.html',
  styleUrl: './warehouse-canvas.css',
})
export class WarehouseCanvas {
  private readonly canvasState = inject(WarehouseCanvasState);
  private readonly containerService = inject(ContainerApiService);
  private readonly itemService = inject(ItemApiService);
  private readonly dialog = inject(MatDialog);

  readonly warehouse = this.canvasState.warehouse;
  readonly path = this.canvasState.path;

  readonly containers = signal<ListContainersQueryDto[]>([]);
  readonly items = signal<ListItemQueryDto[]>([]);
  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  private readonly layout = signal<Record<string, Position>>({});
  private loadToken = 0;

  // The container the card currently being dragged is hovering over, for highlighting.
  private readonly nestTargetId = signal<number | null>(null);

  readonly placedContainers = computed(() => this.containers().filter(c => !!this.layout()[this.key('container', c.id)]));
  readonly unplacedContainers = computed(() => this.containers().filter(c => !this.layout()[this.key('container', c.id)]));
  readonly placedItems = computed(() => this.items().filter(i => !!this.layout()[this.key('item', i.id)]));
  readonly unplacedItems = computed(() => this.items().filter(i => !this.layout()[this.key('item', i.id)]));
  readonly hasUnplaced = computed(() => this.unplacedContainers().length > 0 || this.unplacedItems().length > 0);

  constructor() {
    effect(() => {
      const warehouse = this.warehouse();
      const container = this.canvasState.currentContainer();
      this.canvasState.locationChanged();
      if (!warehouse) {
        this.containers.set([]);
        this.items.set([]);
        this.layout.set({});
        return;
      }
      this.loadLevel(warehouse.id, container ? container.id : null);
    });
  }

  key(kind: EntityKind, id: number): string {
    return `${kind}-${id}`;
  }

  positionOf(kind: EntityKind, id: number): Position {
    return this.layout()[this.key(kind, id)] ?? { x: 0, y: 0 };
  }

  isNestTarget(containerId: number): boolean {
    return this.nestTargetId() === containerId;
  }

  // Bigger container types render bigger cards. Driven off maxContainers (the type's
  // own advertised "c5"/"c10"/"c20" size) rather than the siblings currently on
  // screen, so a given type always renders the same size regardless of what else
  // happens to be visible. Clamped so extreme type configs can't blow up the layout.
  containerScale(container: ListContainersQueryDto): number {
    const scale = 0.75 + container.maxContainers * 0.025;
    return Math.min(1.6, Math.max(0.75, scale));
  }

  itemsFullness(container: ListContainersQueryDto): number {
    return container.maxItems > 0 ? Math.min(1, container.itemQuantityUsed / container.maxItems) : 0;
  }

  containersFullness(container: ListContainersQueryDto): number {
    return container.maxContainers > 0 ? Math.min(1, container.containerCountUsed / container.maxContainers) : 0;
  }

  enterContainer(container: ListContainersQueryDto): void {
    this.canvasState.enterContainer({
      id: container.id,
      name: container.name,
      maxItems: container.maxItems,
      maxContainers: container.maxContainers,
    });
  }

  editContainer(container: ListContainersQueryDto): void {
    const dialogRef = this.dialog.open(ContainerEdit, {
      width: '420px',
      data: {
        id: container.id,
        name: container.name,
        containerTypeId: container.containerTypeId,
        parentContainerId: container.parentContainerId,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.canvasState.notifyLocationChanged();
      }
    });
  }

  deleteContainer(container: ListContainersQueryDto): void {
    const warehouse = this.warehouse();
    if (!warehouse) {
      return;
    }

    const dialogRef = this.dialog.open(ContainerDelete, {
      width: '440px',
      data: {
        id: container.id,
        name: container.name,
        warehouseId: warehouse.id,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.canvasState.notifyLocationChanged();
      }
    });
  }

  deleteItem(item: ListItemQueryDto): void {
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
      this.itemService.delete(item.id).subscribe({
        next: () => this.canvasState.notifyLocationChanged(),
        error: (err) => this.errorMessage.set(extractErrorMessage(err, 'Unable to delete the item.')),
      });
    });
  }

  editItem(item: ListItemQueryDto): void {
    const warehouse = this.warehouse();
    if (!warehouse) {
      return;
    }

    const dialogRef = this.dialog.open(ItemEdit, {
      width: '420px',
      data: {
        id: item.id,
        name: item.name,
        description: item.description,
        quantity: item.quantity,
        supplierId: item.supplier.id,
        containerId: item.container.id,
        warehouseId: warehouse.id,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.canvasState.notifyLocationChanged();
      }
    });
  }

  goToRoot(): void {
    this.canvasState.goToRoot();
  }

  goToCrumb(index: number): void {
    this.canvasState.goToCrumb(index);
  }

  // Fires continuously while a card (placed or still in the tray) is dragged —
  // used only to highlight whatever it's currently hovering over.
  onDragMoved(event: CdkDragMove, kind: EntityKind, id: number, canvasEl: HTMLElement): void {
    const resolution = this.resolveDrop(event.pointerPosition, kind, id, canvasEl);
    this.nestTargetId.set(resolution.action === 'nest' ? resolution.containerId : null);
  }

  // Fires once when a card (placed or still in the tray) is released. This is the
  // single place that decides what a drag actually did — CDK's drop-list events
  // are not used here since they only fire when the release point is inside a
  // connected list, which nest targets are not.
  onDragEnded(event: CdkDragEnd, kind: EntityKind, id: number, canvasEl: HTMLElement, dragRef: CdkDrag): void {
    this.nestTargetId.set(null);
    const resolution = this.resolveDrop(event.dropPoint, kind, id, canvasEl);

    switch (resolution.action) {
      case 'nest':
        this.nestInto(kind, id, resolution.containerId);
        break;
      case 'position':
        this.setPosition(kind, id, resolution.position);
        break;
      case 'outside':
        // Placed cards get removed from the canvas by unplace() below (which
        // destroys/recreates their element in the tray). Tray cards aren't bound to
        // [cdkDragFreeDragPosition], so nothing else resets their drag transform —
        // without this they'd stay visually stuck wherever the pointer let go.
        dragRef.reset();
        if (this.layout()[this.key(kind, id)]) {
          this.unplace(kind, id);
        }
        break;
    }
  }

  unplace(kind: EntityKind, id: number): void {
    const previous = this.layout()[this.key(kind, id)] ?? null;
    const updated = { ...this.layout() };
    delete updated[this.key(kind, id)];
    this.layout.set(updated);
    this.persistPosition(kind, id, null, previous);
  }

  private resolveDrop(point: Position, kind: EntityKind, id: number, canvasEl: HTMLElement): DropResolution {
    const excludeContainerId = kind === 'container' ? id : undefined;

    const nestTargetId = this.findNestTarget(point, excludeContainerId);
    if (nestTargetId != null) {
      return { action: 'nest', containerId: nestTargetId };
    }

    const rect = canvasEl.getBoundingClientRect();
    const insideGrid = point.x >= rect.left && point.x <= rect.right && point.y >= rect.top && point.y <= rect.bottom;
    if (!insideGrid) {
      return { action: 'outside' };
    }

    return {
      action: 'position',
      position: {
        x: Math.max(0, point.x - rect.left - CARD_DROP_OFFSET_X),
        y: Math.max(0, point.y - rect.top - CARD_DROP_OFFSET_Y),
      },
    };
  }

  // elementsFromPoint (not elementFromPoint) because CDK renders a floating preview
  // clone over the pointer while dragging — the real card underneath is further down.
  private elementsAtPoint(point: Position): Element[] {
    return document
      .elementsFromPoint(point.x, point.y)
      .filter(el => !el.classList.contains('cdk-drag-preview') && !el.classList.contains('cdk-drag-placeholder'));
  }

  private findNestTarget(point: Position, excludeContainerId?: number): number | null {
    for (const element of this.elementsAtPoint(point)) {
      const cardEl = element.closest<HTMLElement>('.canvas-card-container');
      if (!cardEl) {
        continue;
      }
      const targetId = Number(cardEl.dataset['containerId']);
      if (Number.isFinite(targetId) && targetId !== excludeContainerId) {
        return targetId;
      }
    }
    return null;
  }

  private nestInto(kind: EntityKind, id: number, targetContainerId: number): void {
    const request = kind === 'container'
      ? this.containerService.moveToContainer(id, targetContainerId)
      : this.itemService.moveToContainer(id, targetContainerId);

    request.subscribe({
      next: () => {
        if (kind === 'container') {
          this.containers.set(
            this.containers()
              .filter(c => c.id !== id)
              .map(c => c.id === targetContainerId ? { ...c, hasChildren: true } : c)
          );
        } else {
          this.items.set(this.items().filter(i => i.id !== id));
        }
        const updated = { ...this.layout() };
        delete updated[this.key(kind, id)];
        this.layout.set(updated);
        this.canvasState.notifyLocationChanged();
      },
      error: (err) => this.errorMessage.set(extractErrorMessage(err, `Unable to move the ${kind} into the container.`)),
    });
  }

  private setPosition(kind: EntityKind, id: number, position: Position): void {
    const previous = this.layout()[this.key(kind, id)] ?? null;
    this.layout.set({ ...this.layout(), [this.key(kind, id)]: position });
    this.persistPosition(kind, id, position, previous);
  }

  private persistPosition(kind: EntityKind, id: number, position: Position | null, previousPosition: Position | null): void {
    const payload = {
      canvasX: position ? position.x : null,
      canvasY: position ? position.y : null,
    };
    const request = kind === 'container'
      ? this.containerService.updateCanvasPosition(id, payload)
      : this.itemService.updateCanvasPosition(id, payload);

    const levelToken = this.loadToken;

    request.subscribe({
      error: () => {
        // Only roll back this entity, and only while the same level is still on screen.
        if (levelToken !== this.loadToken) {
          return;
        }
        const reverted = { ...this.layout() };
        if (previousPosition) {
          reverted[this.key(kind, id)] = previousPosition;
        } else {
          delete reverted[this.key(kind, id)];
        }
        this.layout.set(reverted);
        this.errorMessage.set('Unable to save the layout.');
      },
    });
  }

  private loadLevel(warehouseId: number, containerId: number | null): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    // Cleared together: leaving stale containers/items in place while layout is
    // already reset would make every entity from the level we just left look
    // unplaced (nothing in the fresh, empty layout matches their keys), flashing
    // the tray open with stale cards until the real response lands.
    this.layout.set({});
    this.containers.set([]);
    this.items.set([]);

    // Responses of a level we already navigated away from must not land in the current layout.
    const levelToken = ++this.loadToken;

    this.containerService.list({ warehouseId, parentContainerId: containerId }).subscribe({
      next: containers => {
        if (levelToken !== this.loadToken) {
          return;
        }
        this.containers.set(containers);
        this.mergeLayout('container', containers);
        this.isLoading.set(false);
      },
      error: () => {
        if (levelToken !== this.loadToken) {
          return;
        }
        this.errorMessage.set('Unable to load containers.');
        this.isLoading.set(false);
      },
    });

    if (containerId != null) {
      const query = new ListItemQuery();
      query.containerId = containerId;
      this.itemService.list(query).subscribe({
        next: response => {
          if (levelToken !== this.loadToken) {
            return;
          }
          const items = response.items ?? [];
          this.items.set(items);
          this.mergeLayout('item', items);
        },
        error: () => {
          if (levelToken === this.loadToken) {
            this.items.set([]);
          }
        },
      });
    } else {
      this.items.set([]);
    }
  }

  private mergeLayout(kind: EntityKind, entities: { id: number; canvasX: number | null; canvasY: number | null }[]): void {
    const updated = { ...this.layout() };
    for (const entity of entities) {
      if (entity.canvasX != null && entity.canvasY != null) {
        updated[this.key(kind, entity.id)] = { x: entity.canvasX, y: entity.canvasY };
      }
    }
    this.layout.set(updated);
  }
}
