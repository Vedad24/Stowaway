import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BaseCountdownRedirectComponent } from '../../../../base-classes/base-countdown-component';

@Component({
  selector: 'app-payment-success',
  imports: [],
  templateUrl: './payment-success.html',
  styleUrls: ['./payment-success.css'],
})
export class PaymentSuccess extends BaseCountdownRedirectComponent{
  protected override targetUrl: any[] = ["/main"]; 
  public orderId = signal("0");
  private readonly route = inject(ActivatedRoute);
  override ngOnInit(): void{
    super.ngOnInit();
    this.orderId.set(this.route.snapshot.paramMap.get("orderId")!);
  }
}
