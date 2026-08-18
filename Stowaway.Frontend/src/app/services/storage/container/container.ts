import { Injectable, inject } from '@angular/core';
import { Observable, onErrorResumeNextWith } from 'rxjs';
import { buildHttpParams } from '../../../models/build-http-params';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../enviroments/enivroment';
import { ApiEndpoints } from '../../../shared/constants/api-endpoints';
import { ListContainersQueryResponse, ListContainersQuery, ListContainersQueryDto, MoveContainerCommand, UpdateCanvasPositionCommand, CreateContainerCommand, UpdateContainerCommand, DeleteContainerRequest, ContainerStatusName } from './container.model';

@Injectable({
  providedIn: 'root',
})
export class ContainerApiService {
  private readonly baseUrl = `${environment.apiUrl}/${ApiEndpoints.Container}`;
  private http = inject(HttpClient);

  list(request? : ListContainersQuery): Observable<ListContainersQueryResponse>{
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListContainersQueryResponse>(this.baseUrl, { params });
  }

  getById(id: number): Observable<ListContainersQueryDto>{
    return this.http.get<ListContainersQueryDto>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateContainerCommand): Observable<number>{
    return this.http.post<number>(this.baseUrl, payload);
  }

  update(id: number, payload: UpdateContainerCommand): Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  updateCanvasPosition(id: number, payload: UpdateCanvasPositionCommand): Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${id}/canvas-position`, payload);
  }

  updateStatus(id: number, status: ContainerStatusName): Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${id}/status`, { status });
  }

  moveToContainer(id: number, parentContainerId: number): Observable<void>{
    const payload: MoveContainerCommand = { parentContainerId };
    return this.http.put<void>(`${this.baseUrl}/${id}/parent-container`, payload);
  }

  delete(id: number, request?: DeleteContainerRequest): Observable<void>{
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.delete<void>(`${this.baseUrl}/${id}`, { params });
  }
}
