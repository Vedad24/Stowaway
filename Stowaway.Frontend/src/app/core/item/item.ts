import { Component, inject } from '@angular/core';
import { ItemApiService } from '../../services/storage/item/item';
import { ListItemQuery, ListItemQueryDto, ListItemQueryResponse } from '../../services/storage/item/item.model';
import { BaseListPagedComponent } from '../base-classes/base-list-paged-component';

@Component({
  selector: 'app-item',
  imports: [],
  templateUrl: './item.html',
  styleUrl: './item.css',
})
export class Item
  extends BaseListPagedComponent<ListItemQueryDto, ListItemQuery>
{
  private itemApiService = inject(ItemApiService);

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

}
