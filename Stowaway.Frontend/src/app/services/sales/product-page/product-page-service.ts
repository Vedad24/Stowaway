import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../enviroments/enivroment';
import { ApiEndpoints } from '../../../shared/constants/api-endpoints';
import { buildHttpParams } from '../../../models/build-http-params';
import {
  ListContainerTypeQueryDto,
  ListContainerTypesQuery,
  ListContainerTypesQueryResponse,
  ListWarehousesQuery,
  ListWarehousesQueryDto,
  ListWarehousesQueryResponse,
} from './product-page-service.models';

@Injectable({
  providedIn: 'root',
})
export class ProductPageService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/${ApiEndpoints.ProductPage}`;

  public getUserWarehouses(request?: ListWarehousesQuery): Observable<ListWarehousesQueryResponse> {
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListWarehousesQueryResponse>(`${this.baseUrl}/get-user-warehouses`, {
      params
    });
  }

  public getContainerTypes(request?: ListContainerTypesQuery): Observable<ListContainerTypesQueryResponse> {
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListContainerTypesQueryResponse>(`${this.baseUrl}/get-container-types`, {
      params,
    });
  }
}
