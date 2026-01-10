import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../enviroments/enivroment';
import { ListWarehouseQuery, ListWarehouseQueryResponse } from './warehouse.model';
import { Observable } from 'rxjs';
import { buildHttpParams } from '../../../models/build-http-params';

@Injectable({
  providedIn: 'root',
})
export class WarehouseApiService {
  private readonly baseUrl = `${environment.apiUrl}/Warehouse`;
  private http = inject(HttpClient);

  list(request?: ListWarehouseQuery): Observable<ListWarehouseQueryResponse>{
    const params = request ? buildHttpParams(request as any) : undefined;

    return this.http.get<ListWarehouseQueryResponse>(this.baseUrl, { params });
  }
}
