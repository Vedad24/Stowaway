import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../enviroments/enivroment';
import { ApiEndpoints } from '../../../shared/constants/api-endpoints';
import { Observable } from 'rxjs';
import { CreateOrderCommand, CreateOrderCommandDto, GetOrderByIdQueryDto, ListOrdersQuery, ListOrdersQueryDto, ListOrdersQueryResponse, UpdateOrderCommand } from './order-service.models'
import { buildHttpParams } from '../../../models/build-http-params';
@Injectable({
  providedIn: 'root',
})
export class OrderService {
  http = inject(HttpClient);
  orderUrl = `${environment.apiUrl}/${ApiEndpoints.Order}`;

  public get(id: number): Observable<GetOrderByIdQueryDto> {
    return this.http.get<GetOrderByIdQueryDto>(`${this.orderUrl}/${id}`);
  }

  public list(
    payload: ListOrdersQuery
  ): Observable<ListOrdersQueryResponse> {
    const params = payload ? buildHttpParams(payload) : undefined;
    return this.http.get<ListOrdersQueryResponse>(this.orderUrl, { params });
  }

  public create(
    payload: CreateOrderCommand
  ): Observable<CreateOrderCommandDto> {
    return this.http.post<CreateOrderCommandDto>(this.orderUrl, payload);
  }

  public update(
    payload: UpdateOrderCommand
  ): Observable<boolean> {
    return this.http.put<boolean>(this.orderUrl, payload);
  }

  public delete(id: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.orderUrl}/${id}`);
  }
}
