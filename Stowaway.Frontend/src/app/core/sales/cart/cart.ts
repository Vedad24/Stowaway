import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../../services/sales/cart/cart-service';
import { AddToCartCommand, CartItemDto, CartItemStatus } from '../../../services/sales/cart/cart-service.models';
import { CurrentUserService } from '../../../services/identity/auth/current-user-service';


@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cart.html',
  styleUrl: './cart.css',
})
export class Cart implements OnInit {
  private readonly cartService = inject(CartService);
  private readonly currentUserService = inject(CurrentUserService)
  private get userId() { return this.currentUserService.userId}
  public cartItems = signal<CartItemDto[]>([]);
  isLoading = signal(false);
  message: string | null = null;

  ngOnInit(): void {
    this.loadCart();
  }

  private loadCart(): void {
    this.isLoading.set(true);
    this.cartService.list(this.userId).subscribe({
      next: (response) => {
        this.cartItems.set(response.cartItems ?? []);
        this.isLoading.set(false);
      },
      error: () => {
        this.message = 'Unable to load cart items.';
        this.isLoading.set(false);
      },
    });
  }

  get inCartItems(): CartItemDto[] {
    return this.cartItems().filter((item) => item.cartItemStatus === CartItemStatus.InCart);
  }

  get savedForLaterItems(): CartItemDto[] {
    return this.cartItems().filter((item) => item.cartItemStatus === CartItemStatus.SavedForLater);
  }

  get subtotal(): number {
    return this.inCartItems.reduce((sum, item) => sum + (item.quantity * (item.containerType?.price ?? 0)), 0);
  }

  toggleStatus(item: CartItemDto, targetStatus: CartItemStatus): void {
    const payload : AddToCartCommand = {
      userId: item.userId,
      containerType: item.containerType,
      quantity: item.quantity,
      warehouseId: 0 //change later please.
    };
    
    if (targetStatus === CartItemStatus.InCart) {
      this.cartService.addToCart(payload).subscribe({
        next: () => {
          //item.cartItemStatus = CartItemStatus.InCart;
          this.updateItemStatus(item.id, targetStatus);
          this.message = 'Item moved to In Cart.';
        },
        error: () => {
          this.message = 'Unable to move item to In Cart.';
        },
      });
    }
    else{
      this.cartService.saveForLater(payload).subscribe({
        next: () => {
          //item.cartItemStatus = CartItemStatus.SavedForLater;
          this.updateItemStatus(item.id, targetStatus);
          this.message = 'Item saved for later.';
        },
        error: () => {
          this.message = 'Unable to save item for later.';
        },
      });
    }

  }

  updateItemStatus(itemId : number, newStatus : CartItemStatus) : void
  {
    this.cartItems.update((value) => 
      value.map(item  => 
        item.id === itemId ? 
          {...item, cartItemStatus : newStatus} 
          : item
        )
    )
  }
}
