import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_CONFIG } from '../../../core/config/api-config';
import { CreateWarehouseCommand, GetWarehouseByIdDto, ListWarehouseQuery, ListWarehouseQueryResponse, UpdateWarehouseCommand, UpdateWarehouseNameCommand, UpdateWarehouseNameCommandDto } from './warehouse.model';
import { Observable } from 'rxjs';
import { buildHttpParams } from '../../../models/build-http-params';
import { buildUrl } from '../../../models/build-url';


@Injectable({
  providedIn: 'root',
})
export class WarehouseApiService {
  private readonly config = inject(API_CONFIG);
  private get baseUrl() { return this.config.baseUrl; }
  private http = inject(HttpClient);

  list(request?: ListWarehouseQuery): Observable<ListWarehouseQueryResponse>{
    const params = request ? buildHttpParams(request as any) : undefined;

    return this.http.get<ListWarehouseQueryResponse>(`${this.baseUrl}/${this.config.warehouse.list}`, { params });
  }

  getById(id: number): Observable<GetWarehouseByIdDto>{
    return this.http.get<GetWarehouseByIdDto>(`${this.baseUrl}/${buildUrl(this.config.warehouse.byId, { id })}`);
  }

  create(payload: CreateWarehouseCommand): Observable<number>{
    return this.http.post<number>(`${this.baseUrl}/${this.config.warehouse.create}`, payload);
  }

  update(id: number, payload: UpdateWarehouseCommand): Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${buildUrl(this.config.warehouse.update, { id })}`, payload);
  }

  updateName(id: number, payload: UpdateWarehouseNameCommand): Observable<UpdateWarehouseNameCommandDto>{
    return this.http.patch<UpdateWarehouseNameCommandDto>(`${this.baseUrl}/${buildUrl(this.config.warehouse.updateName, { id })}`, payload);
  }

  delete(id: number): Observable<void>{
    return this.http.delete<void>(`${this.baseUrl}/${buildUrl(this.config.warehouse.delete, { id })}`)
  }
}
