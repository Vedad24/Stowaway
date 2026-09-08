import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { ItemApiService } from '../../services/storage/item/item';
import { ListItemQuery, ListItemQueryDto } from '../../services/storage/item/item.model';
import { BaseListPagedComponent } from '../base-classes/base-list-paged-component';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink} from "@angular/router";

@Component({
  selector: 'app-item',
  imports: [ReactiveFormsModule],
  templateUrl: './item.html',
  styleUrl: './item.css',
})
export class Item
  extends BaseListPagedComponent<ListItemQueryDto, ListItemQuery>
  implements OnInit
{

  private itemApiService = inject(ItemApiService);
  private cdr = inject(ChangeDetectorRef);
  readonly searchControl = new FormControl('', { nonNullable: true });

  constructor(private router: Router) {
    super();
    this.request = new ListItemQuery();
    this.request.paging = { page: 1, pageSize: 10 };
  }

  ngOnInit() {
    this.loadPagedData();
  }

  routeToAdd() {
    this.router.navigate(['/item/create']);
  }

  protected override loadPagedData(): void {
    this.startLoading();

    this.itemApiService.list(this.request).subscribe({
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

  getImageSrc(item: ListItemQueryDto): string | null {
    if (!item.byteImage) return null;
    return `data:image/jpeg;base64,${item.byteImage}`;
  }
  
  searchData() {
    this.request.search = this.searchControl.value;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  deleteItem(id: number) {
    this.itemApiService.delete(id).subscribe({
      next: (request) => {
        this.loadPagedData();
      },
      error: (err) => {
      }
    });
  }

  editItem(id: number) {
    this.router.navigate(['/item/edit',id])
  }
}