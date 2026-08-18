import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../enviroments/enivroment';
import { ApiEndpoints } from '../../../shared/constants/api-endpoints';
import {
  ListSupplierQuery, ListSupplierQueryDto, ListSupplierQueryResponse,
  CreateSupplierCommand, UpdateSupplierCommand, GetSupplierByIdDto
 } from './supplier.model';
import { Observable, retry } from 'rxjs';
import { buildHttpParams } from '../../../models/build-http-params';

@Injectable({
  providedIn: 'root',
})
export class SupplierApiService {
  private readonly baseUrl = `${environment.apiUrl}/${ApiEndpoints.Supplier}`;
  private http = inject(HttpClient);

  list(request?: ListSupplierQuery): Observable<ListSupplierQueryResponse>{
    const params = request ? buildHttpParams(request as any) : undefined;

    return this.http.get<ListSupplierQueryResponse>(this.baseUrl, { params });
  }

  getById(id: number): Observable<ListSupplierQueryDto>{
    return this.http.get<ListSupplierQueryDto>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateSupplierCommand): Observable<number>{
    return this.http.post<number>(this.baseUrl, payload);
  }

  update(id: number, payload: UpdateSupplierCommand): Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void>{
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
