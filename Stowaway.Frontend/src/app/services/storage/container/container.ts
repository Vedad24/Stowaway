import { Injectable, inject } from '@angular/core';
import { Observable, from, onErrorResumeNextWith } from 'rxjs';
import { buildHttpParams } from '../../../models/build-http-params';
import { buildUrl } from '../../../models/build-url';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api-config';
import { ListContainersQueryResponse, ListContainersQuery, ListContainersQueryDto, MoveContainerCommand, UpdateCanvasPositionCommand, CreateContainerCommand, UpdateContainerCommand, DeleteContainerRequest, ContainerStatusName } from './container.model';

const MAX_PAGE_SIZE = 100;

@Injectable({
  providedIn: 'root',
})
export class ContainerApiService {
  private readonly config = inject(API_CONFIG);
  private get baseUrl() { return this.config.baseUrl; }
  private http = inject(HttpClient);

  list(request?: Partial<ListContainersQuery>): Observable<ListContainersQueryResponse>{
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListContainersQueryResponse>(`${this.baseUrl}/${this.config.container.list}`, { params });
  }

  // Fetches every page and concatenates the results. Use for tree walks, dropdown
  // options, and other callers that need the full matching set rather than one page.
  listAll(request?: Partial<ListContainersQuery>): Observable<ListContainersQueryDto[]> {
    return from(this.fetchAllPages(request));
  }

  private async fetchAllPages(request?: Partial<ListContainersQuery>): Promise<ListContainersQueryDto[]> {
    const query = new ListContainersQuery();
    Object.assign(query, request);
    query.paging.pageSize = MAX_PAGE_SIZE;

    const results: ListContainersQueryDto[] = [];
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
