import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_CONFIG } from '../../../core/config/api-config';
import {
  ListSupplierQuery, ListSupplierQueryDto, ListSupplierQueryResponse,
  CreateSupplierCommand, UpdateSupplierCommand, GetSupplierByIdDto
 } from './supplier.model';
import { Observable, retry } from 'rxjs';
import { buildHttpParams } from '../../../models/build-http-params';
import { buildUrl } from '../../../models/build-url';

@Injectable({
  providedIn: 'root',
})
export class SupplierApiService {
  private readonly config = inject(API_CONFIG);
  private get baseUrl() { return this.config.baseUrl; }
  private http = inject(HttpClient);

  list(request?: ListSupplierQuery): Observable<ListSupplierQueryResponse>{
    const params = request ? buildHttpParams(request as any) : undefined;

    return this.http.get<ListSupplierQueryResponse>(`${this.baseUrl}/${this.config.supplier.list}`, { params });
  }

  getById(id: number): Observable<ListSupplierQueryDto>{
    return this.http.get<ListSupplierQueryDto>(`${this.baseUrl}/${buildUrl(this.config.supplier.byId, { id })}`);
  }

  create(payload: CreateSupplierCommand): Observable<number>{
    return this.http.post<number>(`${this.baseUrl}/${this.config.supplier.create}`, payload);
  }

  update(id: number, payload: UpdateSupplierCommand): Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${buildUrl(this.config.supplier.update, { id })}`, payload);
  }

  delete(id: number): Observable<void>{
    return this.http.delete<void>(`${this.baseUrl}/${buildUrl(this.config.supplier.delete, { id })}`);
  }
}
