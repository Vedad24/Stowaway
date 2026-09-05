import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { AutocompleteComponent } from '../../../shared/autocomplete-component/autocomplete-component';
import { OrderService } from '../../../services/sales/order/order-service';
import { ProductPageService } from '../../../services/sales/product-page/product-page-service';
import {
  ListContainerTypeQueryDto,
  ListWarehousesQueryDto,
} from '../../../services/sales/product-page/product-page-service.models';
import { SharedOrderCommandContainerType } from '../../../services/sales/order/order-service.models';

interface OrderItemRow {
  containerTypeId: number;
  warehouseId: number;
  quantity: number;
}

@Component({
  selector: 'app-edit-order',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, AutocompleteComponent],
  templateUrl: './edit-order.html',
  styleUrl: './edit-order.css',
})
export class EditOrder implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly orderService = inject(OrderService);
  private readonly productPageService = inject(ProductPageService);
  private readonly snackBar = inject(MatSnackBar);

  readonly lockedStatuses = ['Completed', 'Cancelled', 'Refunded'];

  orderId!: number;
  orderStatus = signal<string | null>(null);
  isLocked = computed(() => this.lockedStatuses.includes(this.orderStatus() ?? ''));

  warehouses = signal<ListWarehousesQueryDto[]>([]);
  containerTypes = signal<ListContainerTypeQueryDto[]>([]);

  items = new FormArray<FormGroup<{
    containerTypeId: FormControl<number>;
    warehouseId: FormControl<number>;
    quantity: FormControl<number>;
  }>>([]);

  addForm = new FormGroup({
    warehouse: new FormControl<ListWarehousesQueryDto | null>(null, [Validators.required]),
    containerType: new FormControl<number>(0, [Validators.required, Validators.min(1)]),
    quantity: new FormControl<number>(1, [Validators.required, Validators.min(1)]),
  });

  isLoading = signal(false);
  isSaving = signal(false);
  message: string | null = null;

  ngOnInit(): void {
    this.orderId = +this.route.snapshot.params['id'];
    this.loadWarehouses();
    this.loadContainerTypes();
    this.loadOrder();
  }

  private loadWarehouses(): void {
    this.productPageService.getUserWarehouses().subscribe({
      next: (response) => this.warehouses.set(response.items ?? []),
      error: () => (this.message = 'Unable to load warehouses.'),
    });
  }

  private loadContainerTypes(): void {
    this.productPageService.getContainerTypes().subscribe({
      next: (response) => this.containerTypes.set(response.items ?? []),
      error: () => (this.message = 'Unable to load container types.'),
    });
  }

  private loadOrder(): void {
    this.isLoading.set(true);
    this.orderService.get(this.orderId).subscribe({
      next: (response) => {
        this.orderStatus.set(response.orderStatus);
        this.items.clear();
        for (const item of response.items) {
          this.items.push(this.buildRow({
            containerTypeId: item.containerTypeId,
            warehouseId: item.warehouseId,
            quantity: item.quantity,
          }));
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.message = 'Unable to load order.';
        this.isLoading.set(false);
      },
    });
  }

  private buildRow(row: OrderItemRow): FormGroup<{
    containerTypeId: FormControl<number>;
    warehouseId: FormControl<number>;
    quantity: FormControl<number>;
  }> {
    return new FormGroup({
      containerTypeId: new FormControl(row.containerTypeId, { nonNullable: true, validators: [Validators.required] }),
      warehouseId: new FormControl(row.warehouseId, { nonNullable: true, validators: [Validators.required] }),
      quantity: new FormControl(row.quantity, { nonNullable: true, validators: [Validators.required, Validators.min(1)] }),
    });
  }

  containerTypeName(id: number): string {
    return this.containerTypes().find((ct) => ct.id === id)?.displayName ?? 'Unknown';
  }

  containerTypePrice(id: number): number {
    return this.containerTypes().find((ct) => ct.id === id)?.price ?? 0;
  }

  warehouseName(id: number): string {
    return this.warehouses().find((w) => w.id === id)?.name ?? 'Unknown';
  }

  rowTotal(row: FormGroup<{ containerTypeId: FormControl<number>; warehouseId: FormControl<number>; quantity: FormControl<number>; }>): number {
    return row.controls.quantity.value * this.containerTypePrice(row.controls.containerTypeId.value);
  }

  get orderTotal(): number {
    return this.items.controls.reduce((sum, row) => sum + this.rowTotal(row), 0);
  }

  addRow(): void {
    if (this.addForm.invalid) {
      this.addForm.markAllAsTouched();
      this.message = 'Please select a warehouse, container type, and a valid quantity.';
      return;
    }

    const warehouseId = this.addForm.controls.warehouse.value?.id ?? 0;
    const containerTypeId = this.addForm.controls.containerType.value ?? 0;
    const quantity = this.addForm.controls.quantity.value ?? 1;

    const existingRow = this.items.controls.find(
      (row) => row.controls.containerTypeId.value === containerTypeId && row.controls.warehouseId.value === warehouseId,
    );

    if (existingRow) {
      existingRow.controls.quantity.setValue(existingRow.controls.quantity.value + quantity);
    } else {
      this.items.push(this.buildRow({ containerTypeId, warehouseId, quantity }));
    }

    this.message = null;
    this.addForm.reset({ warehouse: null, containerType: 0, quantity: 1 });
  }

  removeRow(index: number): void {
    this.items.removeAt(index);
  }

  save(): void {
    if (this.isLocked()) {
      this.message = `Order is locked once ${this.orderStatus()}.`;
      return;
    }

    if (this.items.length === 0) {
      this.message = 'Orders must have items in them.';
      return;
    }

    if (this.items.invalid) {
      this.items.markAllAsTouched();
      this.message = 'Please fix invalid item rows before saving.';
      return;
    }

    const allContainerTypes: SharedOrderCommandContainerType[] = this.items.controls.map((row) => ({
      containerTypeId: row.controls.containerTypeId.value,
      warehouseId: row.controls.warehouseId.value,
      quantity: row.controls.quantity.value,
    }));

    this.isSaving.set(true);
    this.orderService.update({ id: this.orderId, allContainerTypes }).subscribe({
      next: () => {
        this.router.navigate(['/orders']);
      },
      error: (error) => {
        this.isSaving.set(false);
        this.showSaveError(error?.error?.message);
      },
    });
  }

  private showSaveError(detail?: string): void {
    this.snackBar.open(detail ?? 'Unable to save order. Please try again.', 'Dismiss', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'top',
      panelClass: ['error-snackbar'],
    });
  }

  cancel(): void {
    this.router.navigate(['/orders']);
  }
}
