import { Component, OnInit, inject, signal } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ContainerApiService } from '../../../services/storage/container/container';
import { ProductPageService } from '../../../services/sales/product-page/product-page-service';
import { ListContainerTypeQueryDto } from '../../../services/sales/product-page/product-page-service.models';
import { extractErrorMessage } from '../../../models/http-error';

export interface ContainerAddDialogData {
  warehouseId: number;
  parentContainerId: number | null;
}

@Component({
  selector: 'app-container-add',
  standalone: true,
  imports: [FormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule],
  templateUrl: './container-add.html',
  styleUrl: './container-add.css',
})
export class ContainerAdd implements OnInit {
  readonly dialogRef = inject(MatDialogRef<ContainerAdd>);
  private readonly containerService = inject(ContainerApiService);
  private readonly productPageService = inject(ProductPageService);
  private readonly data = inject(MAT_DIALOG_DATA) as ContainerAddDialogData;

  name = '';
  containerTypeId: number | null = null;
  containerTypes = signal<ListContainerTypeQueryDto[]>([]);
  isLoadingTypes = signal(false);
  errorMessage = signal<string | null>(null);
  isSubmitting = signal(false);

  ngOnInit(): void {
    this.isLoadingTypes.set(true);

    const parentContainerId = this.data.parentContainerId;
    const parent$ = parentContainerId != null ? this.containerService.getById(parentContainerId) : of(null);

    forkJoin([this.productPageService.getContainerTypes(), parent$]).subscribe({
      next: ([typesResponse, parent]) => {
        const allTypes = typesResponse.items ?? [];
        // A container can only ever hold types strictly smaller than its own — same-size
        // or bigger would let it indirectly hold more than its own capacity allows.
        const available = parent
          ? allTypes.filter(t => t.maxItems < parent.maxItems && t.maxContainers < parent.maxContainers)
          : allTypes;

        this.containerTypes.set(available);
        this.isLoadingTypes.set(false);

        if (parent && available.length === 0) {
          this.errorMessage.set('No container type is small enough to fit inside this container.');
        }
      },
      error: (err) => {
        this.isLoadingTypes.set(false);
        this.errorMessage.set(extractErrorMessage(err, 'Unable to load container types.'));
      },
    });
  }

  submit(): void {
    if (!this.name.trim() || this.containerTypeId == null) {
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.containerService.create({
      name: this.name.trim(),
      containerTypeId: this.containerTypeId,
      warehouseId: this.data.warehouseId,
      parentContainerId: this.data.parentContainerId,
    }).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(extractErrorMessage(err, 'Unable to create container. Please try again.'));
      },
    });
  }
}
