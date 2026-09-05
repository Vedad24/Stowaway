import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../../../core/config/api-config';
import {
  CreatePaymentCommand,
  CreatePaymentResponse,
  UpdateStripePaymentCommand,
} from './payment-service.models';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(API_CONFIG);
  private get baseUrl() { return this.config.baseUrl; }

  public pay(payload: CreatePaymentCommand): Observable<CreatePaymentResponse> {
    return this.http.post<CreatePaymentResponse>(`${this.baseUrl}/${this.config.stripePayment.pay}`, payload);
  }

  public paymentWebhook(payload: UpdateStripePaymentCommand): Observable<boolean> {
    return this.http.post<boolean>(`${this.baseUrl}/${this.config.stripePayment.paymentWebhook}`, payload);
  }
}
