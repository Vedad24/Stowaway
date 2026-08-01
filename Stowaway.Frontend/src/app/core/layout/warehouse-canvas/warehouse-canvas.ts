import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, signal } from '@angular/core';
import { CdkDragDrop, CdkDragEnd, CdkDragMove, DragDropModule } from '@angular/cdk/drag-drop';
import { WarehouseCanvasState } from '../../../services/storage/warehouse-canvas-state';
import { ContainerApiService } from '../../../services/storage/container/container';
import { ListContainersQueryDto } from '../../../services/storage/container/container.model';
import { ItemApiService } from '../../../services/storage/item/item';
import { ListItemQuery, ListItemQueryDto } from '../../../services/storage/item/item.model';

interface Position {
  x: number;
  y: number;
}

type EntityKind = 'container' | 'item';
type BreadcrumbTarget = { type: 'root' } | { type: 'container'; id: number };

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

  readonly warehouse = this.canvasState.warehouse;
  readonly path = this.canvasState.path;

  readonly containers = signal<ListContainersQueryDto[]>([]);
  readonly items = signal<ListItemQueryDto[]>([]);
  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  private readonly layout = signal<Record<string, Position>>({});
  private loadToken = 0;
  readonly nestTargetId = signal<number | null>(null);
  readonly breadcrumbTargetId = signal<number | 'root' | null>(null);

  readonly placedContainers = computed(() => this.containers().filter(c => !!this.layout()[this.key('container', c.id)]));
  readonly unplacedContainers = computed(() => this.containers().filter(c => !this.layout()[this.key('container', c.id)]));
  readonly placedItems = computed(() => this.items().filter(i => !!this.layout()[this.key('item', i.id)]));
  readonly unplacedItems = computed(() => this.items().filter(i => !this.layout()[this.key('item', i.id)]));
  readonly hasUnplaced = computed(() => this.unplacedContainers().length > 0 || this.unplacedItems().length > 0);

  readonly emptyDropData: unknown[] = [];

  constructor() {
    effect(() => {
      const warehouse = this.warehouse();
      const container = this.canvasState.currentContainer();
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

  enterContainer(container: ListContainersQueryDto): void {
    this.canvasState.enterContainer({ id: container.id, name: container.name });
  }

  goToRoot(): void {
    this.canvasState.goToRoot();
  }

  goToCrumb(index: number): void {
    this.canvasState.goToCrumb(index);
  }

  onCanvasDrop(event: CdkDragDrop<unknown[]>, canvasEl: HTMLElement): void {
    this.nestTargetId.set(null);
    this.breadcrumbTargetId.set(null);
    if (event.previousContainer === event.container) {
      return;
    }
    const data = event.item.data as { kind: EntityKind; id: number };

    const nestTargetId = this.findNestTarget(event.dropPoint, data.kind === 'container' ? data.id : undefined);
    if (nestTargetId != null) {
      this.nestInto(data.kind, data.id, nestTargetId);
      return;
    }

    const crumbTarget = this.findBreadcrumbTarget(event.dropPoint);
    if (crumbTarget) {
      this.moveToCrumb(data.kind, data.id, crumbTarget);
      return;
    }

    const rect = canvasEl.getBoundingClientRect();
    const x = Math.max(0, event.dropPoint.x - rect.left - 56);
    const y = Math.max(0, event.dropPoint.y - rect.top - 24);
    this.setPosition(data.kind, data.id, { x, y });
  }

  onReposition(event: CdkDragEnd, kind: EntityKind, id: number): void {
    this.nestTargetId.set(null);
    this.breadcrumbTargetId.set(null);

    const nestTargetId = this.findNestTarget(event.dropPoint, kind === 'container' ? id : undefined);
    if (nestTargetId != null) {
      this.nestInto(kind, id, nestTargetId);
      return;
    }

    const crumbTarget = this.findBreadcrumbTarget(event.dropPoint);
    if (crumbTarget) {
      this.moveToCrumb(kind, id, crumbTarget);
      return;
    }

    this.setPosition(kind, id, event.source.getFreeDragPosition());
  }

  onDragMoved(event: CdkDragMove, kind: EntityKind, id: number): void {
    const nestTargetId = this.findNestTarget(event.pointerPosition, kind === 'container' ? id : undefined);
    this.nestTargetId.set(nestTargetId);

    if (nestTargetId != null) {
      this.breadcrumbTargetId.set(null);
      return;
    }

    const crumbTarget = this.findBreadcrumbTarget(event.pointerPosition);
    this.breadcrumbTargetId.set(crumbTarget ? (crumbTarget.type === 'root' ? 'root' : crumbTarget.id) : null);
  }

  private findNestTarget(point: { x: number; y: number }, excludeContainerId?: number): number | null {
    // elementsFromPoint (not elementFromPoint) because CDK renders a floating preview clone
    // over the pointer while dragging — the real card underneath is further down the stack.
    for (const element of document.elementsFromPoint(point.x, point.y)) {
      if (element.classList.contains('cdk-drag-preview') || element.classList.contains('cdk-drag-placeholder')) {
        continue;
      }
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
      error: () => this.errorMessage.set(`Unable to move the ${kind} into the container.`),
    });
  }

  private findBreadcrumbTarget(point: { x: number; y: number }): BreadcrumbTarget | null {
    for (const element of document.elementsFromPoint(point.x, point.y)) {
      if (element.classList.contains('cdk-drag-preview') || element.classList.contains('cdk-drag-placeholder')) {
        continue;
      }
      const crumbEl = element.closest<HTMLElement>('.crumb');
      if (!crumbEl) {
        continue;
      }
      if (crumbEl.dataset['crumbRoot'] !== undefined) {
        return { type: 'root' };
      }
      const containerId = Number(crumbEl.dataset['crumbId']);
      if (crumbEl.dataset['crumbId'] !== undefined && Number.isFinite(containerId)) {
        return { type: 'container', id: containerId };
      }
      return null;
    }
    return null;
  }

  private moveToCrumb(kind: EntityKind, id: number, target: BreadcrumbTarget): void {
    if (kind === 'item' && target.type === 'root') {
      return;
    }

    const request = target.type === 'root'
      ? this.containerService.moveToRoot(id)
      : kind === 'container'
        ? this.containerService.moveToContainer(id, target.id)
        : this.itemService.moveToContainer(id, target.id);

    request.subscribe({
      next: () => {
        if (kind === 'container') {
          this.containers.set(this.containers().filter(c => c.id !== id));
        } else {
          this.items.set(this.items().filter(i => i.id !== id));
        }
        const updated = { ...this.layout() };
        delete updated[this.key(kind, id)];
        this.layout.set(updated);
        this.canvasState.notifyLocationChanged();
      },
      error: () => this.errorMessage.set(`Unable to move the ${kind} back.`),
    });
  }

  unplace(kind: EntityKind, id: number): void {
    const previous = this.layout()[this.key(kind, id)] ?? null;
    const updated = { ...this.layout() };
    delete updated[this.key(kind, id)];
    this.layout.set(updated);
    this.persistPosition(kind, id, null, previous);
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
    this.layout.set({});

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
