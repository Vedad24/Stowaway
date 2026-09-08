import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { SupplierApiService } from '../../services/storage/supplier/supplier';
import {
  ListSupplierQuery, ListSupplierQueryDto
} from '../../services/storage/supplier/supplier.model';
import { BaseListPagedComponent } from '../base-classes/base-list-paged-component';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialog } from '../../shared/confirm-dialog/confirm-dialog';
import { PaginationTable, TableColumnDef } from '../../shared/pagination-table/pagination-table';
import { ThemeToggle } from '../../shared/theme-toggle/theme-toggle';

@Component({
  selector: 'app-supplier',
  imports: [
    PaginationTable,
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    ThemeToggle,
  ],
  templateUrl: './supplier.html',
  styleUrl: './supplier.css',
})
export class Supplier extends BaseListPagedComponent<ListSupplierQueryDto, ListSupplierQuery> implements OnInit {

  private supplierApiService = inject(SupplierApiService);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  private dialog = inject(MatDialog);

  readonly searchControl = new FormControl('', { nonNullable: true });

  columnDef: TableColumnDef<ListSupplierQueryDto>[] = [
    {
      columnDef: 'name',
      header: 'Name',
      cell: (row: ListSupplierQueryDto) => `${row.name}`,
    },
    {
      columnDef: 'description',
      header: 'Description',
      cell: (row: ListSupplierQueryDto) => `${row.description}`,
    },
    {
      columnDef: 'address',
      header: 'Address',
      cell: (row: ListSupplierQueryDto) => `${row.address}`,
    },
    {
      columnDef: 'totalDeliveries',
      header: 'Total Deliveries',
      cell: (row: ListSupplierQueryDto) => `${row.totalDeliveries}`,
    },
    {
      columnDef: 'failedDeliveries',
      header: 'Failed Deliveries',
      cell: (row: ListSupplierQueryDto) => `${row.failedDeliveries}`,
    },
    {
      columnDef: 'btnEdit',
      header: 'Edit',
      type: 'action',
      buttons: [{
        type: 'icon',
        label: 'Edit',
        icon: 'edit',
        action: (row: ListSupplierQueryDto) => this.editItem(row.id),
      }],
    },
    {
      columnDef: 'btnDelete',
      header: 'Delete',
      type: 'action',
      buttons: [{
        type: 'icon',
        label: 'Delete',
        icon: 'delete',
        action: (row: ListSupplierQueryDto) => this.deleteItem(row),
      }],
    },
  ];

  constructor() {
    super();

    this.request = new ListSupplierQuery();
    this.request.paging = { page: 1, pageSize: 10 };
  }

  ngOnInit() {
    this.loadPagedData();
  }

  protected override loadPagedData(): void {
    this.startLoading();

    this.supplierApiService.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error("Load failed:", err);
        this.stopLoading();
        this.cdr.detectChanges();
      }
    });
  }

  searchData() {
    this.request.search = this.searchControl.value;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  deleteItem(row: ListSupplierQueryDto) {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '380px',
      data: {
        title: 'Delete supplier',
        message: `Are you sure you want to delete "${row.name}"? This cannot be undone.`,
        confirmLabel: 'Delete',
        danger: true,
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) {
        return;
      }

      this.supplierApiService.delete(row.id).subscribe({
        next: () => {
          this.loadPagedData();
        },
        error: (err) => {
          console.error("Delete failed:", err);
        }
      });
    });
  }

  routeToAdd() {
    this.router.navigate(['/supplier/create']);
  }

  editItem(id: number) {
    this.router.navigate(['/supplier/edit', id]);
  }
}
