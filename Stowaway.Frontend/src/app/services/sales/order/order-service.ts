import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { API_CONFIG } from '../../../core/config/api-config';
import { Observable } from 'rxjs';
import { CreateOrderCommand, CreateOrderCommandDto, GetOrderByIdQueryDto, ListOrdersQuery, ListOrdersQueryDto, ListOrdersQueryResponse, UpdateOrderCommand } from './order-service.models'
import { buildHttpParams } from '../../../models/build-http-params';
import { buildUrl } from '../../../models/build-url';
@Injectable({
  providedIn: 'root',
})
export class OrderService {
  http = inject(HttpClient);
  private readonly config = inject(API_CONFIG);
  private get baseUrl() { return this.config.baseUrl; }

  public get(id: number): Observable<GetOrderByIdQueryDto> {
    return this.http.get<GetOrderByIdQueryDto>(`${this.baseUrl}/${buildUrl(this.config.order.byId, { id })}`);
  }

  public list(
    payload: ListOrdersQuery
  ): Observable<ListOrdersQueryResponse> {
    const params = payload ? buildHttpParams(payload) : undefined;
    return this.http.get<ListOrdersQueryResponse>(`${this.baseUrl}/${this.config.order.list}`, { params });
  }

  public create(
    payload: CreateOrderCommand
  ): Observable<CreateOrderCommandDto> {
    return this.http.post<CreateOrderCommandDto>(`${this.baseUrl}/${this.config.order.create}`, payload);
  }

  public update(
    payload: UpdateOrderCommand
  ): Observable<boolean> {
    return this.http.put<boolean>(`${this.baseUrl}/${this.config.order.update}`, payload);
  }

  public delete(id: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.baseUrl}/${buildUrl(this.config.order.delete, { id })}`);
  }
}
