import { Component, inject, OnInit } from '@angular/core';
import { SupplierApiService } from '../../services/storage/supplier/supplier';
import {
  ListSupplierQuery, ListSupplierQueryDto, ListSupplierQueryResponse,
  CreateSupplierCommand, GetSupplierByIdDto, UpdateSupplierCommand
 } from '../../services/storage/supplier/supplier.model';
import { BaseListPagedComponent } from '../base-classes/base-list-paged-component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-supplier',
  imports: [FormsModule],
  templateUrl: './supplier.html',
  styleUrl: './supplier.css',
})
export class Supplier
  extends BaseListPagedComponent<ListSupplierQueryDto, ListSupplierQuery>
{
  private supplierApiService = inject(SupplierApiService);
  searchTerm = "";

  constructor() {
    super();
    this.request = new ListSupplierQuery();
  }

  ngOnInit() {
    this.initList();
  }

  protected override loadPagedData(): void {
    this.startLoading();

    this.supplierApiService.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
        console.log(this.items);
        this.totalItems = this.items.length;
      },
      error: (err) => {
        console.log("error " + err.message);
        this.stopLoading();
      }
    })
  }

  searchData() {
    /* this.startLoading(); */

    this.supplierApiService.list(
      {
        paging: {
          page: 1,
          pageSize: 1000
        },
        search: this.searchTerm
      }
    ).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
        console.log(this.items);
        this.totalItems = this.items.length;
      },
      error: (err) => {
        console.log("error " + err.message);
        this.stopLoading();
      }
    })
  }
}
