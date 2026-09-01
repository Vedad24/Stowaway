import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import {
  CreatePriviledgeGroupCommand,
  CreateUpdateWarehouseUserCommand,
  CreateUpdateWarehouseUserCommandDto,
  ListPriviledgeGroupQueryDto,
  UpdatePriviledgeGroupCommand,
} from './priviledge-group-service.models';
import { environment } from '../../../../enviroments/enivroment';
import { ApiEndpoints } from '../../../shared/constants/api-endpoints';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PriviledgeGroupService {
  private readonly http = inject(HttpClient);
  private readonly priviledgeGroupURL = `${environment.apiUrl}/${ApiEndpoints.StorageIdentityPrivilegeGroups}`;
  private readonly warehouseUsersURL = `${environment.apiUrl}/${ApiEndpoints.StorageIdentityWarehouseUsers}`;

  public list(warehouseId: number): Observable<ListPriviledgeGroupQueryDto[]> {
    const params = { warehouseId: warehouseId.toString() };
    return this.http.get<ListPriviledgeGroupQueryDto[]>(this.priviledgeGroupURL, { params });
  }

  public create(payload: CreatePriviledgeGroupCommand): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.priviledgeGroupURL, payload);
  }

  public update(id: number, payload: UpdatePriviledgeGroupCommand): Observable<{ id: number }> {
    return this.http.put<{ id: number }>(`${this.priviledgeGroupURL}/${id}`, payload);
  }

  public delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.priviledgeGroupURL}/${id}`);
  }

  public createUpdateWarehouseUser(
    payload: CreateUpdateWarehouseUserCommand,
  ): Observable<CreateUpdateWarehouseUserCommandDto> {
    return this.http.put<CreateUpdateWarehouseUserCommandDto>(this.warehouseUsersURL, payload);
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
