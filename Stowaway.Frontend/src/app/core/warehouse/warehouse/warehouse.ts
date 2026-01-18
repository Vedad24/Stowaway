import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { WarehouseApiService } from '../../../services/storage/warehouse/warehouse';
import {
  ListWarehouseQuery, ListWarehouseQueryDto
} from '../../../services/storage/warehouse/warehouse.model';
import { BaseListPagedComponent } from '../../base-classes/base-list-paged-component';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-warehouse',
  imports: [FormsModule],
  templateUrl: './warehouse.html',
  styleUrl: './warehouse.css',
})
export class Warehouse
  extends BaseListPagedComponent<ListWarehouseQueryDto, ListWarehouseQuery>
  implements OnInit
{
  private warehouseApiService = inject(WarehouseApiService);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  searchTerm = "";
  
  constructor() {
    super();
    this.request = new ListWarehouseQuery();
    this.request.paging = { page: 1, pageSize: 10 };
  }
  
  ngOnInit() {
    this.loadPagedData();
  }
  
  protected override loadPagedData(): void {
    this.startLoading();
    
    this.warehouseApiService.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err.message);
        this.stopLoading();
        this.cdr.detectChanges();
      }
    });
  }
  
  deleteItem(id: number) {
    this.warehouseApiService.delete(id).subscribe({
      next: (response) => {
        this.loadPagedData();
      },
      error: (err) => {
        console.log(err.message);
      }
    })
  }
  
  searchData() {
    this.request.search = this.searchTerm;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  routeToAdd() {
    this.router.navigate(['/warehouse/create']);
  }
}