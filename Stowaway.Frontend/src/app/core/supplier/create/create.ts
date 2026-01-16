import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SupplierApiService } from '../../../services/storage/supplier/supplier';
import { BaseFormComponent } from '../../base-classes/base-form-component';
import { Router } from '@angular/router';
import { GetSupplierByIdDto, CreateSupplierCommand } from '../../../services/storage/supplier/supplier.model';

@Component({
  selector: 'app-create',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './create.html',
  styleUrl: './create.css',
})
export class CreateSupplier
  extends BaseFormComponent<GetSupplierByIdDto> {
  protected override loadData(): void {
    throw new Error('Method not implemented.');
  }
  protected override save(): void {
    throw new Error('Method not implemented.');
  }
  private fb = inject(FormBuilder);
  private api = inject(SupplierApiService);
  router = inject(Router);

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      totalDeliveries: [0, Validators.required],
      failedDeliveries: [0, Validators.required]
    });
  }

  createItem() {
    this.startLoading();

    const command: CreateSupplierCommand = {
      name: this.form.value.name,
      description: this.form.value.description,
      totalDeliveries: this.form.value.totalDeliveries,
      failedDeliveries: this.form.value.failedDeliveries
    }

    this.api.create(command).subscribe({
      next: (itemId) => {
        this.stopLoading();
        this.router.navigate(['/supplier'])
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

    const formData = new FormData();
    formData.append('name', this.form.value.name);
    formData.append('description', this.form.value.description ?? '');
    formData.append('totalDeliveries', this.form.value.totalDeliveries);
    formData.append('failedDeliveries', this.form.value.failedDeliveries);

    console.log('Submitting item:', this.form.value);
    this.createItem();
  }
}
