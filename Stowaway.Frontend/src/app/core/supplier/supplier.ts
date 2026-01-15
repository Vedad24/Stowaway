import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { SupplierApiService } from '../../services/storage/supplier/supplier';
import {
  ListSupplierQuery, ListSupplierQueryDto
} from '../../services/storage/supplier/supplier.model';
import { BaseListPagedComponent } from '../base-classes/base-list-paged-component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-supplier',
  imports: [FormsModule],
  templateUrl: './supplier.html',
  styleUrl: './supplier.css',
})
export class Supplier extends BaseListPagedComponent<ListSupplierQueryDto, ListSupplierQuery> {
  private supplierApiService = inject(SupplierApiService);
  private cdr = inject(ChangeDetectorRef);
  
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
    // 1. Update the request object parameters
    this.request.search = this.searchTerm;
    this.request.paging.page = 1;
    this.loadPagedData();
  }
}