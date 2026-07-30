import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductPageService } from '../../../services/sales/product-page/product-page-service';
import { CartService } from '../../../services/sales/cart/cart-service';
import {
  ListContainerTypeQueryDto,
  ListWarehousesQueryDto,
} from '../../../services/sales/product-page/product-page-service.models';
import { AddToCartCommand } from '../../../services/sales/cart/cart-service.models';
import { UserService } from '../../../services/identity/user/user-service';
import { CurrentUserService } from '../../../services/identity/auth/current-user-service';
import { AuthService } from '../../../services/identity/auth/auth-service';
import { MatButton, MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-page',
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatButton],
  templateUrl: './product-page.html',
  styleUrl: './product-page.css',
})
export class ProductPage implements OnInit {
  private readonly productPageService = inject(ProductPageService);
  private readonly cartService = inject(CartService);
  private readonly currentUserService = inject(CurrentUserService);
  private readonly router = inject(Router);
  warehouses = signal<ListWarehousesQueryDto[]>([]);
  containerTypes = signal<ListContainerTypeQueryDto[]>([]);

  selectedWarehouseId: number | null = null;
  selectedContainerTypeId: number | null = null;
  quantity = 1;
  isSubmitting = signal(false);
  message: string | null = null;

  ngOnInit(): void {
    this.loadWarehouses();
    this.loadContainerTypes();
    //console.log('Current User ID:', this.currentUserService.userId);
  }

  private loadWarehouses(): void {
    this.productPageService.getUserWarehouses().subscribe({
      next: (response) => {
        this.warehouses.set(response.items ?? []);
        if (this.warehouses().length && this.selectedWarehouseId === null) {
          this.selectedWarehouseId = this.warehouses()[0].id;
        }
      },
      error: () => {
        this.message = 'Unable to load warehouses.';
      },
    });
  }

  private loadContainerTypes(): void {
    this.productPageService.getContainerTypes().subscribe({
      next: (response) => {
        this.containerTypes.set(response.items ?? []);
        if (this.containerTypes().length && this.selectedContainerTypeId === null) {
          this.selectedContainerTypeId = this.containerTypes()[0].id;
        }
      },
      error: () => {
        this.message = 'Unable to load container types.';
      },
    });
  }

  addToCart(): void {
    if (!this.selectedWarehouseId || !this.selectedContainerTypeId || this.quantity <= 0) {
      this.message = 'Please select a warehouse, container type, and a valid quantity.';
      return;
    }

    this.isSubmitting.update(iS => iS = true);
    this.message = null;

    const payload: AddToCartCommand = {
      userId: this.currentUserService.userId,
      containerType: {
        id: this.selectedContainerTypeId,
      },
      quantity: this.quantity,
    };

    this.cartService.addToCart(payload).subscribe({
      next: () => {
        this.message = 'Added to cart successfully.';
        this.isSubmitting.update(iS => iS = false);
      },
      error: () => {
        this.message = 'Failed to add item to cart.';
        this.isSubmitting.update(iS => iS = false);
      },
    });
  }
  goToCart() : void 
  {
    this.router.navigate(['/cart']);
  }
}
