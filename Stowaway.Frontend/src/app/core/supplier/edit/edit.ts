import { ChangeDetectorRef, Component, inject, numberAttribute } from '@angular/core';
import { BaseFormComponent } from '../../base-classes/base-form-component';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

import { GetSupplierByIdDto } from '../../../services/storage/supplier/supplier.model';
import { SupplierApiService } from '../../../services/storage/supplier/supplier';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';


@Component({
  selector: 'app-edit',
  imports: [
    FormsModule,
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
  ],
  templateUrl: './edit.html',
  styleUrl: './edit.css',
})
export class EditSupplier
  extends BaseFormComponent<GetSupplierByIdDto>
{
    private api = inject(SupplierApiService)
    private router = inject(Router)
    private route = inject(ActivatedRoute);
    private cdr = inject(ChangeDetectorRef);
    private supplierDto: GetSupplierByIdDto | null = null;
    private formBuilder = inject(FormBuilder)
    editForm: FormGroup = this.formBuilder.group({
        name: ['', Validators.required],
        description: [''],
        address: [''],
        totalDeliveries: [1, [Validators.required, Validators.min(0)]],
        failedDeliveries: [0, [Validators.required, Validators.min(0)]],
      });
    supplierId!: number;
  
    ngOnInit() {
      this.supplierId = +this.route.snapshot.params['id'];
      this.loadData();
    }
  
    protected override loadData(): void {
      this.startLoading();
  
      this.api.getById(this.supplierId).subscribe({
        next: (response) => {
          this.supplierDto = response;
          this.editForm.patchValue(response);
          this.stopLoading();
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.stopLoading();
          this.cdr.detectChanges();
        }
      })
    }
    protected override save(): void {
      if (this.editForm.invalid) {
        this.editForm.markAllAsTouched();
        return;
      }
  
      const payload = this.editForm.getRawValue();
      this.api.update(this.supplierId, payload).subscribe({
        next: (response) => {
          this.router.navigate(['/supplier']);
        },
        error: (err) => {
        }
      })
    }
}
