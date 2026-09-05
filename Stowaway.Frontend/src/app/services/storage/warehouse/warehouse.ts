import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../enviroments/enivroment';
import { ApiEndpoints } from '../../../shared/constants/api-endpoints';
import { CreateWarehouseCommand, GetWarehouseByIdDto, ListWarehouseQuery, ListWarehouseQueryResponse, UpdateWarehouseCommand, UpdateWarehouseNameCommand, UpdateWarehouseNameCommandDto } from './warehouse.model';
import { Observable } from 'rxjs';
import { buildHttpParams } from '../../../models/build-http-params';


@Injectable({
  providedIn: 'root',
})
export class WarehouseApiService {
  private readonly baseUrl = `${environment.apiUrl}/${ApiEndpoints.Warehouse}`;
  private http = inject(HttpClient);

  list(request?: ListWarehouseQuery): Observable<ListWarehouseQueryResponse>{
    const params = request ? buildHttpParams(request as any) : undefined;

    return this.http.get<ListWarehouseQueryResponse>(this.baseUrl, { params });
  }

  getById(id: number): Observable<GetWarehouseByIdDto>{
    return this.http.get<GetWarehouseByIdDto>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateWarehouseCommand): Observable<number>{
    return this.http.post<number>(this.baseUrl, payload);
  }

  update(id: number, payload: UpdateWarehouseCommand): Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  updateName(id: number, payload: UpdateWarehouseNameCommand): Observable<UpdateWarehouseNameCommandDto>{
    return this.http.patch<UpdateWarehouseNameCommandDto>(`${this.baseUrl}/${id}/name`, payload);
  }

  delete(id: number): Observable<void>{
    return this.http.delete<void>(`${this.baseUrl}/${id}`)
  }
}
