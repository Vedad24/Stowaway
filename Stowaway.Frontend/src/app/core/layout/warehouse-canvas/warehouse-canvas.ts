import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, signal } from '@angular/core';
import { CdkDragDrop, CdkDragEnd, DragDropModule } from '@angular/cdk/drag-drop';
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
    if (event.previousContainer === event.container) {
      return;
    }
    const data = event.item.data as { kind: EntityKind; id: number };
    const rect = canvasEl.getBoundingClientRect();
    const x = Math.max(0, event.dropPoint.x - rect.left - 56);
    const y = Math.max(0, event.dropPoint.y - rect.top - 24);
    this.setPosition(data.kind, data.id, { x, y });
  }

  onReposition(event: CdkDragEnd, kind: EntityKind, id: number): void {
    this.setPosition(kind, id, event.source.getFreeDragPosition());
  }

  unplace(kind: EntityKind, id: number): void {
    const warehouse = this.warehouse();
    if (!warehouse) {
      return;
    }
    const updated = { ...this.layout() };
    delete updated[this.key(kind, id)];
    this.layout.set(updated);
    this.writeLayout(warehouse.id, this.currentContainerId(), updated);
  }

  private setPosition(kind: EntityKind, id: number, position: Position): void {
    const warehouse = this.warehouse();
    if (!warehouse) {
      return;
    }
    const updated = { ...this.layout(), [this.key(kind, id)]: position };
    this.layout.set(updated);
    this.writeLayout(warehouse.id, this.currentContainerId(), updated);
  }

  private currentContainerId(): number | null {
    const container = this.canvasState.currentContainer();
    return container ? container.id : null;
  }

  private loadLevel(warehouseId: number, containerId: number | null): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.layout.set(this.readLayout(warehouseId, containerId));

    this.containerService.list({ warehouseId, parentContainerId: containerId }).subscribe({
      next: containers => {
        this.containers.set(containers);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load containers.');
        this.isLoading.set(false);
      },
    });

    if (containerId != null) {
      const query = new ListItemQuery();
      query.containerId = containerId;
      this.itemService.list(query).subscribe({
        next: response => this.items.set(response.items ?? []),
        error: () => this.items.set([]),
      });
    } else {
      this.items.set([]);
    }
  }

  private layoutStorageKey(warehouseId: number, containerId: number | null): string {
    return `stowaway.warehouse-canvas.${warehouseId}.${containerId ?? 'root'}`;
  }

  private readLayout(warehouseId: number, containerId: number | null): Record<string, Position> {
    try {
      const raw = localStorage.getItem(this.layoutStorageKey(warehouseId, containerId));
      return raw ? JSON.parse(raw) : {};
    } catch {
      return {};
    }
  }

  private writeLayout(warehouseId: number, containerId: number | null, layout: Record<string, Position>): void {
    try {
      localStorage.setItem(this.layoutStorageKey(warehouseId, containerId), JSON.stringify(layout));
    } catch {
      // ignore storage failures (e.g. private browsing quota)
    }
  }
}
