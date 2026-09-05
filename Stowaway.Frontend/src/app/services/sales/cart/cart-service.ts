import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api-config';
import { buildUrl } from '../../../models/build-url';
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
  private readonly config = inject(API_CONFIG);
  private get baseUrl() { return this.config.baseUrl; }

  public list(userId: number): Observable<ListCartItemsQueryDto> {
    return this.http.get<ListCartItemsQueryDto>(`${this.baseUrl}/${buildUrl(this.config.cart.byUserId, { userId })}`);
  }

  public addToCart(payload: AddToCartCommand): Observable<AddToCartCommandDto> {
    return this.http.post<AddToCartCommandDto>(`${this.baseUrl}/${this.config.cart.addToCart}`, payload);
  }

  public saveForLater(payload: SaveForLaterCommand): Observable<SaveForLaterCommandDto> {
    return this.http.post<SaveForLaterCommandDto>(`${this.baseUrl}/${this.config.cart.saveForLater}`, payload);
  }

  public clearCart(userId: number): Observable<ClearCartCommandDto> {
    return this.http.delete<ClearCartCommandDto>(`${this.baseUrl}/${buildUrl(this.config.cart.clearCart, { userId })}`);
  }
}
