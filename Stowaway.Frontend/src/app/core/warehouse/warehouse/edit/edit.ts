import { Component, inject, numberAttribute } from '@angular/core';
import { BaseFormComponent } from '../../../base-classes/base-form-component';
import { GetWarehouseByIdDto } from '../../../../services/storage/warehouse/warehouse.model';
import { WarehouseApiService } from '../../../../services/storage/warehouse/warehouse';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit',
  imports: [FormsModule, CommonModule, ReactiveFormsModule],
  templateUrl: './edit.html',
  styleUrl: './edit.css',
})
export class EditWarehouse
  extends BaseFormComponent<GetWarehouseByIdDto>
{
  private api = inject(WarehouseApiService)
  private router = inject(Router)
  private route = inject(ActivatedRoute);
  private warehouseDto: GetWarehouseByIdDto | null = null;
  private formBuilder = inject(FormBuilder)
  editForm: FormGroup = this.formBuilder.group({
      name: ['', Validators.required],
      description: [''],
      city: [''],
      address: [''],
      capacity: [1, [Validators.required, Validators.min(0)]],
      isEnabled: [null],
    });
  warehouseId!: number;

  ngOnInit() {
    this.warehouseId = +this.route.snapshot.params['abc'];
    this.loadData();
  }

  protected override loadData(): void {
    this.startLoading();

    this.api.getById(this.warehouseId).subscribe({
      next: (response) => {
        this.warehouseDto = response;
        this.editForm.patchValue(response);
        this.stopLoading();
      },
      error: (err) => {
        console.log(err.message);
        this.stopLoading();
      }
    })
  }
  protected override save(): void {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    const payload = this.editForm.getRawValue();
    this.api.update(this.warehouseId, payload).subscribe({
      next: (response) => {
        this.router.navigate(['/warehouse']);
      },
      error: (err) => {
        console.log(err.message);
      }
    })
  }

}
