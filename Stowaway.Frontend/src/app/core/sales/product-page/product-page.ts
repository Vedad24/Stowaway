import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { AutocompleteComponent } from '../../../shared/autocomplete-component/autocomplete-component';
import { ProductPageService } from '../../../services/sales/product-page/product-page-service';
import { CartService } from '../../../services/sales/cart/cart-service';
import {
  ListContainerTypeQueryDto,
  ListWarehousesQueryDto,
} from '../../../services/sales/product-page/product-page-service.models';
import { AddToCartCommand } from '../../../services/sales/cart/cart-service.models';
import { CurrentUserService } from '../../../services/identity/auth/current-user-service';
import { AuthService } from '../../../services/identity/auth/auth-service';
import { MatButton } from '@angular/material/button';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    AutocompleteComponent,
    MatButton
  ],
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

  productForm = new FormGroup({
    warehouse: new FormControl<ListWarehousesQueryDto | null>(null, [Validators.required]),
    containerType: new FormControl<number>(0, [Validators.required, Validators.min(1)]),
    quantity: new FormControl<number>(1, [Validators.required, Validators.min(1)]),
  });

  isSubmitting = signal(false);
  message: string | null = null;

  ngOnInit(): void {
    this.loadWarehouses();
    this.loadContainerTypes();
  }

  private loadWarehouses(): void {
    this.productPageService.getUserWarehouses().subscribe({
      next: (response) => {
        this.warehouses.set(response.items ?? []);
        if (this.warehouses().length && !this.productForm.get('warehouse')?.value) {
          this.productForm.patchValue({ warehouse: this.warehouses()[0] });
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
        if (this.containerTypes().length && this.productForm.get('containerType')?.value === 0) {
          this.productForm.patchValue({ containerType: this.containerTypes()[0].id });
        }
      },
      error: () => {
        this.message = 'Unable to load container types.';
      },
    });
  }

  addToCart(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      this.message = 'Please select a warehouse, container type, and a valid quantity.';
      return;
    }

    const warehouseId = this.productForm.get('warehouse')?.value?.id ?? 0;
    const containerTypeId = this.productForm.get('containerType')?.value ?? 0;
    const quantity = this.productForm.get('quantity')?.value ?? 0;

    this.isSubmitting.update(() => true);
    this.message = null;

    const payload: AddToCartCommand = {
      userId: this.currentUserService.userId,
      warehouseId,
      containerType: {
        id: containerTypeId,
      },
      quantity,
    };

    this.cartService.addToCart(payload).subscribe({
      next: () => {
        this.message = 'Added to cart successfully.';
        this.isSubmitting.update(() => false);
      },
      error: () => {
        this.message = 'Failed to add item to cart.';
        this.isSubmitting.update(() => false);
      },
    });
  }
  goToCart() : void 
  {
    this.router.navigate(['/cart']);
  }
}
