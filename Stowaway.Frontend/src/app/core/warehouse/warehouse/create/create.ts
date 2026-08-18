import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { WarehouseApiService } from '../../../../services/storage/warehouse/warehouse';
import { BaseFormComponent } from '../../../base-classes/base-form-component';
import { Router } from '@angular/router';
import { GetWarehouseByIdDto, CreateWarehouseCommand } from '../../../../services/storage/warehouse/warehouse.model';


@Component({
  selector: 'app-create',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './create.html',
  styleUrl: './create.css',
})
export class CreateWarehouse
  extends BaseFormComponent<GetWarehouseByIdDto>
{
  private fb = inject(FormBuilder);
    private api = inject(WarehouseApiService);
    router = inject(Router);
  
    ngOnInit(): void {
      this.form = this.fb.group({
        name: ['', Validators.required],
        description: [''],
        city: [''],
        address: [''],
        capacity: [1, Validators.required],
        isEnabled: [true],
      });
    }
  
    createItem() {
      this.startLoading();
  
      const command: CreateWarehouseCommand = {
        name: this.form.value.name,
        description: this.form.value.description,
        city: this.form.value.city,
        address: this.form.value.address,
        capacity: this.form.value.capacity,
        isEnabled: this.form.value.isEnabled
      }
  
      this.api.create(command).subscribe({
        next: (itemId) => {
          this.stopLoading();
          this.router.navigate(['/warehouse'])
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
      formData.append('city', this.form.value.city ?? '');
      formData.append('address', this.form.value.address ?? '');
      formData.append('capacity', this.form.value.capacity ?? '');
      formData.append('isEnabled', this.form.value.isEnabled ?? '');
  
      this.createItem();
    }

  protected override loadData(): void {
    throw new Error('Method not implemented.');
  }
  protected override save(): void {
    throw new Error('Method not implemented.');
  }

}
