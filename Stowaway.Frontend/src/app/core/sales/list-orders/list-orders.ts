import { Component, inject, ChangeDetectorRef, OnInit } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatDialog } from '@angular/material/dialog';
import { OrderService } from '../../../services/sales/order/order-service';
import { ListOrdersQuery, ListOrdersQueryDto, OrderStatus } from '../../../services/sales/order/order-service.models';
import { PaginationTable, TableColumnDef } from '../../../shared/pagination-table/pagination-table';
import { BaseListPagedComponent } from '../../base-classes/base-list-paged-component';
import { CurrentUserService } from '../../../services/identity/auth/current-user-service';
import { ConfirmDialog } from '../../../shared/confirm-dialog/confirm-dialog';
import { RoleName } from '../../../services/identity/user/user-service.models';

@Component({
  selector: 'app-list-orders',
  imports: [PaginationTable, FormsModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, MatDatepickerModule],
  providers: [DatePipe, CurrencyPipe, provideNativeDateAdapter()],
  templateUrl: './list-orders.html',
  styleUrl: './list-orders.css',
})
export class ListOrders extends BaseListPagedComponent<ListOrdersQueryDto, ListOrdersQuery> implements OnInit {
  private readonly orderService = inject(OrderService);
  private readonly dialog = inject(MatDialog);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly datePipe = inject(DatePipe);
  private readonly currencyPipe = inject(CurrencyPipe);
  readonly currentUserService = inject(CurrentUserService);
  readonly roleName = RoleName;

  readonly statusOptions = [
    { value: OrderStatus.Draft, label: 'Draft' },
    { value: OrderStatus.Processing, label: 'Processing' },
    { value: OrderStatus.Completed, label: 'Completed' },
    { value: OrderStatus.Cancelled, label: 'Cancelled' },
    { value: OrderStatus.Refunded, label: 'Refunded' },
  ];

  // Search fields, bound to their own backend params (ANDed server-side, not a unified search box)
  searchByUserEmail = '';
  searchByUserName = '';
  searchByStatus: OrderStatus | null = null;
  createTimeMin: Date | null = null;
  createTimeMax: Date | null = null;

  columnDef: TableColumnDef<ListOrdersQueryDto>[] = [
    {
      columnDef: 'name',
      header: 'Name',
      cell: (row: ListOrdersQueryDto) => `${row.user.name}`,
    },
    {
      columnDef: 'email',
      header: 'Email',
      cell: (row: ListOrdersQueryDto) => `${row.user.email}`,
    },
    {
      columnDef: 'orderDate',
      header: 'Order Date',
      cell: (row: ListOrdersQueryDto) => `${this.datePipe.transform(row.orderDate, 'short')}`,
    },
    {
      columnDef: 'status',
      header: 'Order Status',
      cell: (row: ListOrdersQueryDto) => `${row.orderStatus}`,
    },
    {
      columnDef: 'subtotal',
      header: 'Subtotal',
      cell: (row: ListOrdersQueryDto) => `${this.currencyPipe.transform(row.subtotal)}`,
    },
    {
      columnDef: 'total',
      header: 'Total',
      cell: (row: ListOrdersQueryDto) => `${this.currencyPipe.transform(row.total)}`,
    },
  ];

  constructor() {
    super();
    this.request = new ListOrdersQuery();

    // Edit/Delete are admin-only; a per-viewer condition, so built once here rather than per-row.
    if (this.currentUserService.isAdmin) {
      this.columnDef.push(
        {
          columnDef: 'btnEdit',
          header: 'Edit',
          type: 'action',
          buttons: [
            {
              type: 'icon',
              label: 'Edit',
              icon: 'edit',
              color: '',
              action: (row: ListOrdersQueryDto) => this.onEdit(row),
            },
          ],
        },
        {
          columnDef: 'btnDelete',
          header: 'Delete',
          type: 'action',
          buttons: [
            {
              type: 'icon',
              label: 'Delete',
              icon: 'delete',
              color: '',
              action: (row: ListOrdersQueryDto) => this.onDelete(row),
            },
          ],
        },
      );
    }
  }

  ngOnInit(): void {
    this.loadPagedData();
  }

  protected override loadPagedData(): void {
    this.startLoading();

    this.orderService.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error loading orders', error);
        this.stopLoading();
        this.cdr.detectChanges();
      },
    });
  }

  onSearch(): void {
    this.request.searchByUserEmail = this.searchByUserEmail || null;
    this.request.searchByUserName = this.searchByUserName || null;
    this.request.searchByStatus = this.searchByStatus;
    this.request.createTimeMin = this.createTimeMin;
    this.request.createTimeMax = this.createTimeMax;
    this.paging.page = 1;
    this.loadPagedData();
  }

  onEdit(row: ListOrdersQueryDto): void {
    // TODO: build the order edit UI/flow
  }

  onDelete(row: ListOrdersQueryDto): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '380px',
      data: {
        title: 'Delete order',
        message: `Are you sure you want to delete order for "${row.user.name}"? This cannot be undone.`,
        confirmLabel: 'Delete',
        danger: true,
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) {
        return;
      }

      this.orderService.delete(row.id).subscribe({
        next: () => {
          this.loadPagedData();
        },
        error: () => {
          console.error('There was an error deleting the order');
        },
      });
    });
  }
}
