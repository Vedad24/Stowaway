import { HttpClient } from '@angular/common/http';
import { inject, Injectable, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Observable, of } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api-config';
import { buildHttpParams } from '../../../models/build-http-params';
import {
  ListContainerTypeQueryDto,
  ListContainerTypesQuery,
  ListContainerTypesQueryResponse,
  ListWarehousesQuery,
  ListWarehousesQueryDto,
  ListWarehousesQueryResponse,
} from './product-page-service.models';

// SSR has no auth token to call get-user-warehouses (requires authenticated user) -> serve this placeholder, real list loads after hydration in-browser.
const MOCK_WAREHOUSES_RESPONSE: ListWarehousesQueryResponse = {
  items: [{ id: 0, name: 'Loading warehouses…' }],
  total: 1,
  totalPages: 1,
};

@Injectable({
  providedIn: 'root',
})
export class ProductPageService {
  private readonly http = inject(HttpClient);
  private readonly platformId = inject(PLATFORM_ID);
  private readonly config = inject(API_CONFIG);
  private get baseUrl() { return this.config.baseUrl; }

  public getUserWarehouses(request?: ListWarehousesQuery): Observable<ListWarehousesQueryResponse> {
    if (!isPlatformBrowser(this.platformId)) {
      return of(MOCK_WAREHOUSES_RESPONSE);
    }
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListWarehousesQueryResponse>(`${this.baseUrl}/${this.config.productPage.getUserWarehouses}`, {
      params
    });
  }

  public getContainerTypes(request?: ListContainerTypesQuery): Observable<ListContainerTypesQueryResponse> {
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListContainerTypesQueryResponse>(`${this.baseUrl}/${this.config.productPage.getContainerTypes}`, {
      params,
    });
  }
}
