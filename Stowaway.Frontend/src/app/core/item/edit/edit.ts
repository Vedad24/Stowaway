import { Component, inject, numberAttribute } from '@angular/core';
import { BaseFormComponent } from '../../base-classes/base-form-component';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { GetSupplierByIdDto } from '../../../services/storage/supplier/supplier.model';
import { SupplierApiService } from '../../../services/storage/supplier/supplier';
import { GetItemByIdDto } from '../../../services/storage/item/item.model';
import { ItemApiService } from '../../../services/storage/item/item';
import { ContainerApiService } from '../../../services/storage/container/container';

@Component({
  selector: 'app-edit',
  imports: [FormsModule, CommonModule, ReactiveFormsModule],
  templateUrl: './edit.html',
  styleUrl: './edit.css',
})
export class EditItem
  extends BaseFormComponent<GetItemByIdDto> {
  private api = inject(ItemApiService)
  private supplierService = inject(SupplierApiService);
  private containerService = inject(ContainerApiService)
  private router = inject(Router)
  private route = inject(ActivatedRoute);
  private itemDto: GetItemByIdDto | null = null;
  private formBuilder = inject(FormBuilder)
  editForm: FormGroup = this.formBuilder.group({
    name: ['', Validators.required],
    description: [''],
    byteImage: [null],
    quantity: [1, [Validators.required, Validators.min(0)]],
    supplierId: [null],
    containerId: [null]
  });

  itemId!: number;
  suppliers: any[] = [];
  containers: any[] = [];
  selectedImageFile: File | null = null;

  ngOnInit() {
    this.itemId = +this.route.snapshot.params['id'];
    this.loadContainers();
    this.loadSuppliers();
    this.loadData();
  }

  loadSuppliers() {
    this.supplierService.list().subscribe(res => {
      this.suppliers = res.items;
      this.tryPatchForm();
    });
  }

  loadContainers() {
    this.containerService.list().subscribe(res => {
      this.containers = res;
      this.tryPatchForm();
    });
  }

  protected override loadData(): void {
    this.startLoading();

    this.api.getById(this.itemId).subscribe({
      next: (response) => {
        this.itemDto = response;
        this.tryPatchForm();
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading();
      }
    })
  }

  private tryPatchForm() {
    if (!this.itemDto) return;
    if (this.suppliers.length === 0) return;
    if (this.containers.length === 0) return;

    this.editForm.patchValue({
      name: this.itemDto.name,
      description: this.itemDto.description,
      byteImage: this.itemDto.byteImage,
      quantity: this.itemDto.quantity,
      supplierId: this.itemDto.supplier.id,
      containerId: this.itemDto.container.id
    });
  }

  protected override save(): void {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    const payload = this.editForm.getRawValue();
    this.api.update(this.itemId, payload).subscribe({
      next: (response) => {
        this.router.navigate(['/item']);
      },
      error: (err) => {
      }
    })
  }

  onFileSelected(event: Event) {
  const input = event.target as HTMLInputElement;
  if (input.files && input.files.length > 0) {
    this.selectedImageFile = input.files[0];
    const reader = new FileReader();

    reader.onload = () => {
      const base64 = (reader.result as string).split(',')[1];
      this.editForm.patchValue({ byteImage: base64 });
    }

    reader.readAsDataURL(this.selectedImageFile);
  }
}

}
