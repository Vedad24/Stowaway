import { Component, inject, OnInit } from '@angular/core';
import { WarehouseApiService } from '../../../services/storage/warehouse/warehouse';
import {
  ListWarehouseQuery, ListWarehouseQueryDto, ListWarehouseQueryResponse,
  GetWarehouseByIdDto, CreateWarehouseCommand, UpdateWarehouseCommand
} from '../../../services/storage/warehouse/warehouse.model';
import { BaseListPagedComponent } from '../../base-classes/base-list-paged-component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-warehouse',
  imports: [FormsModule],
  templateUrl: './warehouse.html',
  styleUrl: './warehouse.css',
})
export class Warehouse
  extends BaseListPagedComponent<ListWarehouseQueryDto, ListWarehouseQuery>{
  
  private warehouseApiService = inject(WarehouseApiService);
  searchTerm = "";

  constructor() {
    super();
    this.request = new ListWarehouseQuery();
  }
  
  ngOnInit(){
    this.initList();
  }
  
  //Loading all based on request
  protected override loadPagedData(): void {
    this.startLoading();

    this.warehouseApiService.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
        console.log(this.items);
        this.totalItems = this.items.length;
      },
      error: (err) => {
        console.log(err.message)
        this.stopLoading();
      }
    })
  }

  searchData() {
    this.request.search = this.searchTerm;
    this.request.paging.page = 1;
    this.loadData();
  }

}
