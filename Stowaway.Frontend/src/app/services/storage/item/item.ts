import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_CONFIG } from '../../../core/config/api-config';
import { CreateItemCommand, GetItemByIdDto, ListItemQuery, ListItemQueryDto, ListItemQueryResponse, MoveItemCommand, SetFavouriteCommand, UpdateCanvasPositionCommand, UpdateItemCommand } from './item.model';
import { Observable, from, firstValueFrom, onErrorResumeNextWith } from 'rxjs';
import { buildHttpParams } from '../../../models/build-http-params';
import { buildUrl } from '../../../models/build-url';
import { observableToBeFn } from 'rxjs/internal/testing/TestScheduler';

const MAX_PAGE_SIZE = 100;

@Injectable({
  providedIn: 'root',
})
export class ItemApiService {
  private readonly config = inject(API_CONFIG);
  private get baseUrl() { return this.config.baseUrl; }
  private http = inject(HttpClient);

  list(request? : ListItemQuery): Observable<ListItemQueryResponse>{
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListItemQueryResponse>(`${this.baseUrl}/${this.config.item.list}`, { params });
  }

  // Fetches every page and concatenates the results. Use for sidebar/canvas trees
  // and other callers that need the full matching set rather than one page.
  listAll(request?: ListItemQuery): Observable<ListItemQueryDto[]> {
    return from(this.fetchAllPages(request));
  }

  private async fetchAllPages(request?: ListItemQuery): Promise<ListItemQueryDto[]> {
    const query = new ListItemQuery();
    Object.assign(query, request);
    query.paging.pageSize = MAX_PAGE_SIZE;

    const results: ListItemQueryDto[] = [];
    let page = 1;

    while (true) {
      query.paging.page = page;
      const response = await firstValueFrom(this.list(query));
      results.push(...(response.items ?? []));

      if (page >= (response.totalPages ?? 1)) {
        break;
      }
      page++;
    }

    return results;
  }

  getById(id: number): Observable<GetItemByIdDto>{
    return this.http.get<GetItemByIdDto>(`${this.baseUrl}/${buildUrl(this.config.item.byId, { id })}`);
  }

  create(payload: CreateItemCommand): Observable<number>{
    return this.http.post<number>(`${this.baseUrl}/${this.config.item.create}`, payload);
  }

  update(id: number, payload: UpdateItemCommand): Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${buildUrl(this.config.item.update, { id })}`, payload);
  }

  delete(id: number): Observable<void>{
    return this.http.delete<void>(`${this.baseUrl}/${buildUrl(this.config.item.delete, { id })}`);
  }

  updateCanvasPosition(id: number, payload: UpdateCanvasPositionCommand): Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${buildUrl(this.config.item.canvasPosition, { id })}`, payload);
  }

  moveToContainer(id: number, containerId: number): Observable<void>{
    const payload: MoveItemCommand = { containerId };
    return this.http.put<void>(`${this.baseUrl}/${buildUrl(this.config.item.moveToContainer, { id })}`, payload);
  }

  setFavourite(id: number, isFavourite: boolean): Observable<void>{
    const payload: SetFavouriteCommand = { isFavourite };
    return this.http.put<void>(`${this.baseUrl}/${buildUrl(this.config.item.favourite, { id })}`, payload);
  }
}
