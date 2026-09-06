import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { WarehouseApiService } from '../../services/storage/warehouse/warehouse';
import { ContainerApiService } from '../../services/storage/container/container';
import { ItemApiService } from '../../services/storage/item/item';
import { ListItemQuery } from '../../services/storage/item/item.model';
import { extractErrorMessage } from '../../models/http-error';
import { ThemeToggle } from '../../shared/theme-toggle/theme-toggle';
import { TruncatePipe } from '../../shared/pipes/truncate.pipe';

export type WarehouseReportType = 'containers' | 'items' | 'both';

interface ContainerReportRow {
  id: number;
  name: string;
  ancestorPath: string;
  fullPath: string;
  typeName: string;
  itemsUsed: number;
  maxItems: number;
  containersUsed: number;
  maxContainers: number;
  status: string;
}

interface ItemReportRow {
  id: number;
  name: string;
  containerPath: string;
  quantity: number;
  supplierName: string;
  tags: string;
}

type SortDirection = 'asc' | 'desc';

interface SortState<T extends string> {
  column: T;
  direction: SortDirection;
}

type ContainerSortColumn = 'name' | 'ancestorPath' | 'typeName' | 'itemsUsed' | 'containersUsed' | 'status';
type ItemSortColumn = 'name' | 'containerPath' | 'quantity' | 'supplierName' | 'tags';

const MAX_PAGE_SIZE = 100;

@Component({
  selector: 'app-warehouse-report',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, ThemeToggle, TruncatePipe],
  templateUrl: './warehouse-report.html',
  styleUrl: './warehouse-report.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WarehouseReport implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly titleService = inject(Title);
  private readonly warehouseService = inject(WarehouseApiService);
  private readonly containerService = inject(ContainerApiService);
  private readonly itemService = inject(ItemApiService);

  private warehouseId!: number;

  warehouseName = signal('');
  reportType = signal<WarehouseReportType>('both');
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);
  generatedAt = signal<Date | null>(null);

  containerRows = signal<ContainerReportRow[]>([]);
  itemRows = signal<ItemReportRow[]>([]);

  containerSort = signal<SortState<ContainerSortColumn> | null>(null);
  itemSort = signal<SortState<ItemSortColumn> | null>(null);

  sortedContainerRows = computed(() => this.sortRows(this.containerRows(), this.containerSort()));
  sortedItemRows = computed(() => this.sortRows(this.itemRows(), this.itemSort()));

  get showContainers(): boolean {
    return this.reportType() !== 'items';
  }

  get showItems(): boolean {
    return this.reportType() !== 'containers';
  }

  ngOnInit(): void {
    this.warehouseId = Number(this.route.snapshot.paramMap.get('warehouseId'));
    const requestedType = this.route.snapshot.queryParamMap.get('type') as WarehouseReportType | null;
    this.reportType.set(requestedType === 'containers' || requestedType === 'items' ? requestedType : 'both');

    this.loadReport();
  }

  print(): void {
    window.print();
  }

  goBack(): void {
    this.router.navigate(['/main']);
  }

  toggleContainerSort(column: ContainerSortColumn): void {
    this.containerSort.set(this.toggleSort(this.containerSort(), column));
  }

  toggleItemSort(column: ItemSortColumn): void {
    this.itemSort.set(this.toggleSort(this.itemSort(), column));
  }

  private toggleSort<T extends string>(current: SortState<T> | null, column: T): SortState<T> {
    if (current?.column === column) {
      return { column, direction: current.direction === 'asc' ? 'desc' : 'asc' };
    }
    return { column, direction: 'asc' };
  }

  private sortRows<Row, K extends keyof Row & string>(rows: Row[], sort: SortState<K> | null): Row[] {
    if (!sort) {
      return rows;
    }

    const { column, direction } = sort;
    const factor = direction === 'asc' ? 1 : -1;

    return [...rows].sort((a, b) => {
      const left = a[column];
      const right = b[column];

      if (typeof left === 'number' && typeof right === 'number') {
        return (left - right) * factor;
      }

      return String(left).localeCompare(String(right), undefined, { numeric: true, sensitivity: 'base' }) * factor;
    });
  }

  private async loadReport(): Promise<void> {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      const warehouse = await firstValueFrom(this.warehouseService.getById(this.warehouseId));
      this.warehouseName.set(warehouse.name);
      this.titleService.setTitle(`${warehouse.name} report`);

      const containerRows = await this.collectContainers(this.warehouseId, null, []);
      this.containerRows.set(containerRows);

      if (this.showItems) {
        const itemRows: ItemReportRow[] = [];
        for (const row of containerRows) {
          if (row.itemsUsed <= 0) {
            continue;
          }
          const rows = await this.collectItemsForContainer(row.id, row.fullPath);
          itemRows.push(...rows);
        }
        this.itemRows.set(itemRows);
      }

      this.generatedAt.set(new Date());
    } catch (err) {
      this.errorMessage.set(extractErrorMessage(err, 'Unable to generate the report. Please try again.'));
    } finally {
      this.isLoading.set(false);
    }
  }

  private async collectContainers(
    warehouseId: number,
    parentContainerId: number | null,
    ancestorNames: string[],
  ): Promise<ContainerReportRow[]> {
    const children = await firstValueFrom(this.containerService.list({ warehouseId, parentContainerId }));
    const rows: ContainerReportRow[] = [];

    for (const container of children) {
      const ancestorPath = ancestorNames.join(' / ');
      const fullPath = ancestorPath ? `${ancestorPath} / ${container.name}` : container.name;

      rows.push({
        id: container.id,
        name: container.name,
        ancestorPath: ancestorPath || '—',
        fullPath,
        typeName: container.containerTypeName,
        itemsUsed: container.itemQuantityUsed,
        maxItems: container.maxItems,
        containersUsed: container.containerCountUsed,
        maxContainers: container.maxContainers,
        status: container.currentStatus?.name ?? '—',
      });

      if (container.hasChildren) {
        const childRows = await this.collectContainers(warehouseId, container.id, [...ancestorNames, container.name]);
        rows.push(...childRows);
      }
    }

    return rows;
  }

  private async collectItemsForContainer(containerId: number, containerPath: string): Promise<ItemReportRow[]> {
    const rows: ItemReportRow[] = [];
    let page = 1;

    while (true) {
      const query = new ListItemQuery();
      query.containerId = containerId;
      query.paging.page = page;
      query.paging.pageSize = MAX_PAGE_SIZE;

      const result = await firstValueFrom(this.itemService.list(query));
      for (const item of result.items ?? []) {
        rows.push({
          id: item.id,
          name: item.name,
          containerPath,
          quantity: item.quantity,
          supplierName: item.supplier?.name ?? '—',
          tags: (item.tags ?? []).map(tag => tag.name).join(', '),
        });
      }

      if (page >= (result.totalPages ?? 1)) {
        break;
      }
      page++;
    }

    return rows;
  }
}
