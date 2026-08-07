import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ItemApiService } from '../../../services/storage/item/item';
import { ContainerTreeService } from '../../../services/storage/container/container-tree';
import { SupplierApiService } from '../../../services/storage/supplier/supplier';
import { ListSupplierQueryDto } from '../../../services/storage/supplier/supplier.model';
import { DynamicForm } from '../../../shared/dynamic-form/dynamic-form';
import { DynamicFieldConfig } from '../../../shared/dynamic-form/dynamic-form.models';

export interface ItemEditDialogData {
  id: number;
  name: string;
  description: string;
  quantity: number;
  supplierId: number;
  containerId: number;
  warehouseId: number;
}

interface ItemFormValue {
  name: string;
  description: string;
  quantity: number;
  supplierId: number | null;
  containerId: number | null;
}

@Component({
  selector: 'app-item-edit',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, DynamicForm],
  templateUrl: './item-edit.html',
  styleUrl: './item-edit.css',
})
export class ItemEdit implements OnInit {
  readonly dialogRef = inject(MatDialogRef<ItemEdit>);
  private readonly itemService = inject(ItemApiService);
  private readonly containerTree = inject(ContainerTreeService);
  private readonly supplierService = inject(SupplierApiService);
  private readonly data = inject(MAT_DIALOG_DATA) as ItemEditDialogData;

  formValue = signal<ItemFormValue>({
    name: this.data.name,
    description: this.data.description,
    quantity: this.data.quantity,
    supplierId: this.data.supplierId,
    containerId: this.data.containerId,
  });

  containerOptions = signal<{ id: number; label: string }[]>([]);
  suppliers = signal<ListSupplierQueryDto[]>([]);
  isLoadingOptions = signal(false);
  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);

  readonly fields = computed<DynamicFieldConfig[]>(() => [
    { key: 'name', label: 'Name', type: 'text', required: true },
    { key: 'description', label: 'Description', type: 'textarea', rows: 2 },
    { key: 'quantity', label: 'Quantity', type: 'number', required: true, min: 0 },
    {
      key: 'supplierId',
      label: 'Supplier',
      type: 'select',
      required: true,
      disabled: this.isLoadingOptions(),
      options: this.suppliers().map(s => ({ value: s.id, label: s.name })),
    },
    {
      key: 'containerId',
      label: 'Place (container)',
      type: 'select',
      required: true,
      disabled: this.isLoadingOptions(),
      options: this.containerOptions().map(c => ({ value: c.id, label: c.label })),
    },
  ]);

  readonly isValid = computed(() => {
    const v = this.formValue();
    return !!v.name?.trim() && v.containerId != null && v.supplierId != null && v.quantity != null && v.quantity >= 0;
  });

  ngOnInit(): void {
    this.isLoadingOptions.set(true);
    Promise.all([
      this.containerTree.loadOptions(this.data.warehouseId),
      firstValueFrom(this.supplierService.list()),
    ]).then(([containerOptions, supplierResponse]) => {
      this.containerOptions.set(containerOptions);
      this.suppliers.set(supplierResponse.items ?? []);
      this.isLoadingOptions.set(false);
    }).catch(() => {
      this.isLoadingOptions.set(false);
      this.errorMessage.set('Unable to load containers or suppliers.');
    });
  }

  submit(): void {
    if (!this.isValid()) {
      return;
    }

    const v = this.formValue();
    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.itemService.update(this.data.id, {
      name: v.name.trim(),
      description: (v.description ?? '').trim(),
      quantity: v.quantity,
      supplierId: v.supplierId!,
      containerId: v.containerId!,
    }).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.dialogRef.close(true);
      },
      error: () => {
        this.isSubmitting.set(false);
        this.errorMessage.set('Unable to save changes. Please try again.');
      },
    });
  }
}
