import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { SupplierApiService } from '../../services/storage/supplier/supplier';
import {
  ListSupplierQuery, ListSupplierQueryDto
} from '../../services/storage/supplier/supplier.model';
import { BaseListPagedComponent } from '../base-classes/base-list-paged-component';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-supplier',
  imports: [FormsModule],
  templateUrl: './supplier.html',
  styleUrl: './supplier.css',
})
export class Supplier extends BaseListPagedComponent<ListSupplierQueryDto, ListSupplierQuery> {

  private supplierApiService = inject(SupplierApiService);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  
  searchTerm = "";

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
    this.request.search = this.searchTerm;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  deleteItem(id: number) {
    this.supplierApiService.delete(id).subscribe({
      next: (request) => {
        this.loadPagedData();
      },
      error: (err) => {
      }
    });
  }

   routeToAdd() {
    this.router.navigate(['/supplier/create']);
  }

  editItem(id: number) {
    this.router.navigate(['/supplier/edit', id],)
  }
}