import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_CONFIG } from '../../../core/config/api-config';
import { CreateItemCommand, GetItemByIdDto, ListItemQuery, ListItemQueryDto, ListItemQueryResponse, MoveItemCommand, UpdateCanvasPositionCommand, UpdateItemCommand } from './item.model';
import { Observable, onErrorResumeNextWith } from 'rxjs';
import { buildHttpParams } from '../../../models/build-http-params';
import { buildUrl } from '../../../models/build-url';
import { observableToBeFn } from 'rxjs/internal/testing/TestScheduler';

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
}
