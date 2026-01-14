import { Component, inject } from '@angular/core';
import { ItemApiService } from '../../services/storage/item/item';
import { ListItemQuery, ListItemQueryDto, ListItemQueryResponse } from '../../services/storage/item/item.model';
import { BaseListPagedComponent } from '../base-classes/base-list-paged-component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-item',
  imports: [FormsModule],
  templateUrl: './item.html',
  styleUrl: './item.css',
})
export class Item
  extends BaseListPagedComponent<ListItemQueryDto, ListItemQuery>
{
  private itemApiService = inject(ItemApiService);
  searchTerm = "";

  constructor() {
    super();
    this.request = new ListItemQuery();
  }

  protected override loadPagedData(): void {
    this.startLoading();

    this.itemApiService.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
        console.log(this.items);
      },
      error: (err) => {
        console.log(err.message);
        this.stopLoading();
      }
    })
  }

  ngOnInit() {
    this.initList();
  }

  searchData() {
    this.itemApiService.list(
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

