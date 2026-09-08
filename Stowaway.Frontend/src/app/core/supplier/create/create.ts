import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SupplierApiService } from '../../../services/storage/supplier/supplier';
import { BaseFormComponent } from '../../base-classes/base-form-component';
import { Router, RouterLink } from '@angular/router';
import { GetSupplierByIdDto, CreateSupplierCommand } from '../../../services/storage/supplier/supplier.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-create',
  imports: [
    ReactiveFormsModule,
    CommonModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
  ],
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
      address: [''],
      totalDeliveries: [0, Validators.required],
      failedDeliveries: [0, Validators.required]
    });
  }

  createItem() {
    this.startLoading();

    const command: CreateSupplierCommand = {
      name: this.form.value.name,
      description: this.form.value.description,
      address: this.form.value.address,
      totalDeliveries: this.form.value.totalDeliveries,
      failedDeliveries: this.form.value.failedDeliveries
    }

    this.api.create(command).subscribe({
      next: (itemId) => {
        this.stopLoading();
        this.router.navigate(['/supplier'])
      },
      error: (err) => {
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
    formData.append('address', this.form.value.address ?? '');
    formData.append('totalDeliveries', this.form.value.totalDeliveries);
    formData.append('failedDeliveries', this.form.value.failedDeliveries);

    this.createItem();
  }
}
