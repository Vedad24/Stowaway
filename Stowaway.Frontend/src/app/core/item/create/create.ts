import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SupplierApiService } from '../../../services/storage/supplier/supplier';
import { CreateItemCommand, GetItemByIdDto } from '../../../services/storage/item/item.model';
import { ItemApiService } from '../../../services/storage/item/item';
import { BaseFormComponent } from '../../base-classes/base-form-component';
import { Router } from '@angular/router';
import { ContainerApiService } from '../../../services/storage/container/container';


@Component({
  selector: 'app-create',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './create.html',
  styleUrl: './create.css',
})
export class CreateItem
  extends BaseFormComponent<GetItemByIdDto>
{
  protected override loadData(): void {
    throw new Error('Method not implemented.');
  }
  protected override save(): void {
    throw new Error('Method not implemented.');
  }


private fb = inject(FormBuilder);
  private supplierService = inject(SupplierApiService);
  private containerService = inject(ContainerApiService);
  private api = inject(ItemApiService);
  router = inject(Router);

  suppliers: any[] = [];
  containers: any[] = [];

  selectedImageFile: File | null = null;

  ngOnInit(): void {
    this.loadSuppliers();
    this.loadContainers();
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      image: [null],
      quantity: [1, Validators.required],
      supplierId: [null, Validators.required],
      containerId: [null, Validators.required],
    });
  }

  loadSuppliers() {
    this.supplierService.list().subscribe(res => {
      this.suppliers = res.items;
    });
  }

  loadContainers() {
    this.containerService.list().subscribe(res => {
      this.containers = res;
    });
    console.log(this.containers);
  }

  onImageSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedImageFile = input.files[0];
      const reader = new FileReader();

      reader.onload = () => {
        const base64 = (reader.result as string).split(',')[1];
        this.form.patchValue({ image: base64 });
      }

      reader.readAsDataURL(this.selectedImageFile);
      //this.form.patchValue({ image: this.selectedImageFile });
      //console.log(this.selectedImageFile);
    }
  }

  createItem() {
    this.startLoading();

    const command: CreateItemCommand = {
      name: this.form.value.name,
      description: this.form.value.description,
      byteImage: this.form.value.image,
      quantity: this.form.value.quantity,
      supplierId: this.form.value.supplierId,
      containerId: this.form.value.containerId
    }

    this.api.create(command).subscribe({
      next: (itemId) => {
        this.stopLoading();
        this.router.navigate(['/item'])
        console.log(command);
      },
      error: (err) => {
        console.log(err.message);
      }
    })
    
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    /* const formData = new FormData();
    formData.append('name', this.form.value.name);
    formData.append('description', this.form.value.description ?? '');
    formData.append('quantity', this.form.value.quantity);
    formData.append('supplierId', this.form.value.supplierId);
    formData.append('containerId', this.form.value.containerId);
    if (this.selectedImageFile) {
      formData.append('image', this.form.value.image);
    } */

    console.log('Submitting item:', this.form.value);
    this.createItem();
  }
}
