import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../enviroments/enivroment';
import { ApiEndpoints } from '../../../shared/constants/api-endpoints';
import { CreateItemCommand, GetItemByIdDto, ListItemQuery, ListItemQueryDto, ListItemQueryResponse, MoveItemCommand, UpdateCanvasPositionCommand, UpdateItemCommand } from './item.model';
import { Observable, onErrorResumeNextWith } from 'rxjs';
import { buildHttpParams } from '../../../models/build-http-params';
import { observableToBeFn } from 'rxjs/internal/testing/TestScheduler';

@Injectable({
  providedIn: 'root',
})
export class ItemApiService {
  private readonly baseUrl = `${environment.apiUrl}/${ApiEndpoints.Item}`;
  private http = inject(HttpClient);

  list(request? : ListItemQuery): Observable<ListItemQueryResponse>{
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListItemQueryResponse>(this.baseUrl, { params });
  }

  getById(id: number): Observable<GetItemByIdDto>{
    return this.http.get<GetItemByIdDto>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateItemCommand): Observable<number>{
    return this.http.post<number>(this.baseUrl, payload);
  }

  update(id: number, payload: UpdateItemCommand): Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void>{
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  updateCanvasPosition(id: number, payload: UpdateCanvasPositionCommand): Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${id}/canvas-position`, payload);
  }

  moveToContainer(id: number, containerId: number): Observable<void>{
    const payload: MoveItemCommand = { containerId };
    return this.http.put<void>(`${this.baseUrl}/${id}/container`, payload);
  }
}
