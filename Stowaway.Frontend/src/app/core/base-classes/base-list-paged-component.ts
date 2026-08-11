import { PageResult } from '../../models/paging/page-result';
import {BaseListComponent} from './base-list-component';
import {BasePagedQuery} from '../../models/paging/base-paged-query';
import { signal } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';

export abstract class BaseListPagedComponent<TDto, TRequest extends BasePagedQuery>
  extends BaseListComponent<TDto> {

  constructor() {
    super();
  }

  request!: TRequest;
  totalItems = signal(0);
  totalPages = signal(0);

  get paging(){
    return this.request.paging;
  }

  protected abstract loadPagedData(): void;

  protected override loadData(): void {
    this.loadPagedData();
  }

  protected handlePageResult(result: PageResult<TDto>) {
    this.items.set(result.items);
    this.totalItems.set(result.total);
    this.totalPages.set(result.totalPages);
  }

  goToPage(page: number): void {
    if (page < 1 || (this.totalPages() && page > this.totalPages())) return;
    this.paging.page = page;
    this.loadPagedData();
  }

  nextPage() { this.goToPage(this.paging.page + 1); }
  prevPage() { this.goToPage(this.paging.page - 1); }

  changePageSize(size: number) {
    this.paging.pageSize = size;
    this.paging.page = 1;
    this.loadPagedData();
  }

  onPage(event: PageEvent): void {
    console.log("Page event registered", event);
    if (event.pageSize !== this.paging.pageSize) {
      this.changePageSize(event.pageSize);
    } else {
      this.goToPage(event.pageIndex + 1);
    }
  }
}
