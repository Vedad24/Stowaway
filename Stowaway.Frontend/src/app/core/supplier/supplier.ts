import { Component, inject, OnInit } from '@angular/core';
import { SupplierApiService } from '../../services/storage/supplier/supplier';
import {
  ListSupplierQuery, ListSupplierQueryDto, ListSupplierQueryResponse,
  CreateSupplierCommand, GetSupplierByIdDto, UpdateSupplierCommand
 } from '../../services/storage/supplier/supplier.model';
import { BaseListPagedComponent } from '../base-classes/base-list-paged-component';

@Component({
  selector: 'app-supplier',
  imports: [],
  templateUrl: './supplier.html',
  styleUrl: './supplier.css',
})
export class Supplier
  extends BaseListPagedComponent<ListSupplierQueryDto, ListSupplierQuery>
{
  private supplierApiService = inject(SupplierApiService);

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
      },
      error: (err) => {
        console.log("error " + err.message);
        this.stopLoading();
      }
    })
  }

}
