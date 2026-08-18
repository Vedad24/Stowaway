import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../enviroments/enivroment';
import { ApiEndpoints } from '../../../shared/constants/api-endpoints';
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
  private readonly baseUrl = `${environment.apiUrl}/${ApiEndpoints.StripePayment}`;

  public pay(payload: CreatePaymentCommand): Observable<CreatePaymentResponse> {
    return this.http.post<CreatePaymentResponse>(`${this.baseUrl}/pay`, payload);
  }

  public paymentWebhook(payload: UpdateStripePaymentCommand): Observable<boolean> {
    return this.http.post<boolean>(`${this.baseUrl}/payment-webhook`, payload);
  }
}
