import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../enviroments/enivroment';
import { ApiEndpoints } from '../../../shared/constants/api-endpoints';
import {
  AddToCartCommand,
  AddToCartCommandDto,
  ClearCartCommandDto,
  ListCartItemsQueryDto,
  SaveForLaterCommand,
  SaveForLaterCommandDto,
} from './cart-service.models';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/${ApiEndpoints.Cart}`;

  public list(userId: number): Observable<ListCartItemsQueryDto> {
    return this.http.get<ListCartItemsQueryDto>(`${this.baseUrl}/${userId}`);
  }

  public addToCart(payload: AddToCartCommand): Observable<AddToCartCommandDto> {
    return this.http.post<AddToCartCommandDto>(`${this.baseUrl}/add-to-cart`, payload);
  }

  public saveForLater(payload: SaveForLaterCommand): Observable<SaveForLaterCommandDto> {
    return this.http.post<SaveForLaterCommandDto>(`${this.baseUrl}/save-for-later`, payload);
  }

  public clearCart(userId: number): Observable<ClearCartCommandDto> {
    return this.http.delete<ClearCartCommandDto>(`${this.baseUrl}/clear-cart/${userId}`);
  }
}
