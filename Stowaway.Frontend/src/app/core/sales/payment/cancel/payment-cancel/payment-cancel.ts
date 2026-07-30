import { Component } from '@angular/core';
import { BaseCountdownRedirectComponent } from '../../../../base-classes/base-countdown-component';

@Component({
  selector: 'app-payment-cancel',
  imports: [],
  templateUrl: './payment-cancel.html',
  styleUrls: ['./payment-cancel.css'],
})
export class PaymentCancel extends BaseCountdownRedirectComponent {
  protected override targetUrl: any[] = ['/main'];
}
