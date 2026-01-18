import { Component, inject, OnInit } from '@angular/core';
import { WarehouseApiService } from '../../../services/storage/warehouse/warehouse';
import { ListWarehouseQuery, ListWarehouseQueryDto, ListWarehouseQueryResponse } from '../../../services/storage/warehouse/warehouse.model';
import { BaseListPagedComponent } from '../../base-classes/base-list-paged-component';
import { RouterLink } from "@angular/router";
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-main-section',
  imports: [RouterLink, MatButtonModule],
  templateUrl: './main-section.html',
  styleUrl: './main-section.css',
})
export class MainSection
  extends BaseListPagedComponent<ListWarehouseQueryDto, ListWarehouseQuery>
  implements OnInit {
  
  protected override loadPagedData(): void {
    this.startLoading();

    this.warehouseService.list(this.request).subscribe({
      next: (data) => {
        this.handlePageResult(data);
        this.stopLoading();
      },
      error: err => {
        this.errorMessage = "Failed to load";
        this.stopLoading();
      }
    },
    )
  }

  private warehouseService = inject(WarehouseApiService);
  constructor() {
    super();
    this.request = new ListWarehouseQuery();
  }


  ngOnInit() {
    this.initList();
  }
  
  onSearchChange(searchTerm: string): void {
    this.request.search = searchTerm;
    this.request.paging.page = 1;
    this.loadPagedData();
  }
}

