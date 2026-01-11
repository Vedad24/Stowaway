import { Component, inject, OnInit } from '@angular/core';
import { WarehouseApiService } from '../../../services/storage/warehouse/warehouse';
import {
  ListWarehouseQuery, ListWarehouseQueryDto, ListWarehouseQueryResponse,
  GetWarehouseByIdDto, CreateWarehouseCommand, UpdateWarehouseCommand
} from '../../../services/storage/warehouse/warehouse.model';
import { BaseListPagedComponent } from '../../base-classes/base-list-paged-component';


@Component({
  selector: 'app-warehouse',
  imports: [],
  templateUrl: './warehouse.html',
  styleUrl: './warehouse.css',
})
export class Warehouse
  extends BaseListPagedComponent<ListWarehouseQueryDto, ListWarehouseQuery>{
  
  private warehouseApiService = inject(WarehouseApiService);

  constructor() {
    super();
    this.request = new ListWarehouseQuery();
  }
  
  ngOnInit(){
    this.initList();
  }
  
  protected override loadPagedData(): void {
    this.startLoading();

    this.warehouseApiService.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
        console.log(this.items);
      },
      error: (err) => {
        console.log(err.message)
        this.stopLoading();
      }
    })
  }

}
