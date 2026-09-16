import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import {
  CreatePriviledgeGroupCommand,
  CreateUpdateWarehouseUserCommand,
  CreateUpdateWarehouseUserCommandDto,
  ListPriviledgeGroupQueryResponse,
  ListPriviledgeGroupsQuery,
  UpdatePriviledgeGroupCommand,
} from './priviledge-group-service.models';
import { API_CONFIG } from '../../../core/config/api-config';
import { buildUrl } from '../../../models/build-url';
import { buildHttpParams } from '../../../models/build-http-params';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PriviledgeGroupService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(API_CONFIG);

  public list(query: ListPriviledgeGroupsQuery): Observable<ListPriviledgeGroupQueryResponse> {
    const params = buildHttpParams(query as any);
    return this.http.get<ListPriviledgeGroupQueryResponse>(`${this.config.baseUrl}/${this.config.storageIdentity.privilegeGroups.list}`, { params });
  }

  public create(payload: CreatePriviledgeGroupCommand): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${this.config.baseUrl}/${this.config.storageIdentity.privilegeGroups.create}`, payload);
  }

  public update(id: number, payload: UpdatePriviledgeGroupCommand): Observable<{ id: number }> {
    return this.http.put<{ id: number }>(`${this.config.baseUrl}/${buildUrl(this.config.storageIdentity.privilegeGroups.byId, { id })}`, payload);
  }

  public delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.config.baseUrl}/${buildUrl(this.config.storageIdentity.privilegeGroups.byId, { id })}`);
  }

  public createUpdateWarehouseUser(
    payload: CreateUpdateWarehouseUserCommand,
  ): Observable<CreateUpdateWarehouseUserCommandDto> {
    return this.http.put<CreateUpdateWarehouseUserCommandDto>(`${this.config.baseUrl}/${this.config.storageIdentity.warehouseUsers}`, payload);
  }

  priviledgeGroupData: {
    name?: string;
    description?: string;
  } = {};

  setData(data: { name: string; description: string }) {
    this.priviledgeGroupData = data;
  }

  clearData() {
    this.priviledgeGroupData = {};
  }
}
