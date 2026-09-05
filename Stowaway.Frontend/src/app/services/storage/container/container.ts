import { Injectable, inject } from '@angular/core';
import { Observable, onErrorResumeNextWith } from 'rxjs';
import { buildHttpParams } from '../../../models/build-http-params';
import { buildUrl } from '../../../models/build-url';
import { HttpClient } from '@angular/common/http';
import { API_CONFIG } from '../../../core/config/api-config';
import { ListContainersQueryResponse, ListContainersQuery, ListContainersQueryDto, MoveContainerCommand, UpdateCanvasPositionCommand, CreateContainerCommand, UpdateContainerCommand, DeleteContainerRequest, ContainerStatusName } from './container.model';

@Injectable({
  providedIn: 'root',
})
export class ContainerApiService {
  private readonly config = inject(API_CONFIG);
  private get baseUrl() { return this.config.baseUrl; }
  private http = inject(HttpClient);

  list(request? : ListContainersQuery): Observable<ListContainersQueryResponse>{
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListContainersQueryResponse>(`${this.baseUrl}/${this.config.container.list}`, { params });
  }

  getById(id: number): Observable<ListContainersQueryDto>{
    return this.http.get<ListContainersQueryDto>(`${this.baseUrl}/${buildUrl(this.config.container.byId, { id })}`);
  }

  create(payload: CreateContainerCommand): Observable<number>{
    return this.http.post<number>(`${this.baseUrl}/${this.config.container.create}`, payload);
  }

  update(id: number, payload: UpdateContainerCommand): Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${buildUrl(this.config.container.update, { id })}`, payload);
  }

  updateCanvasPosition(id: number, payload: UpdateCanvasPositionCommand): Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${buildUrl(this.config.container.canvasPosition, { id })}`, payload);
  }

  updateStatus(id: number, status: ContainerStatusName): Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${buildUrl(this.config.container.status, { id })}`, { status });
  }

  moveToContainer(id: number, parentContainerId: number): Observable<void>{
    const payload: MoveContainerCommand = { parentContainerId };
    return this.http.put<void>(`${this.baseUrl}/${buildUrl(this.config.container.parentContainer, { id })}`, payload);
  }

  delete(id: number, request?: DeleteContainerRequest): Observable<void>{
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.delete<void>(`${this.baseUrl}/${buildUrl(this.config.container.delete, { id })}`, { params });
  }
}
