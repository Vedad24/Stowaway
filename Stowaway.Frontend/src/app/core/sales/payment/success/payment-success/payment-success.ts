import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-payment-success',
  imports: [],
  templateUrl: './payment-success.html',
  styleUrl: './payment-success.css',
})
export class PaymentSuccess implements OnInit{
  public orderId = signal("0");
  private readonly route = inject(ActivatedRoute);
  ngOnInit(): void {
    this.orderId.set(this.route.snapshot.paramMap.get("orderId")!);
  }
}
