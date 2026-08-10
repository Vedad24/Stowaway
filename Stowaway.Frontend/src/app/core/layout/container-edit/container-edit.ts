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

export interface ContainerEditDialogData {
  id: number;
  name: string;
  containerTypeId: number;
  parentContainerId: number | null;
}

@Component({
  selector: 'app-container-edit',
  standalone: true,
  imports: [FormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule],
  templateUrl: './container-edit.html',
  styleUrl: './container-edit.css',
})
export class ContainerEdit implements OnInit {
  readonly dialogRef = inject(MatDialogRef<ContainerEdit>);
  private readonly containerService = inject(ContainerApiService);
  private readonly productPageService = inject(ProductPageService);
  private readonly data = inject(MAT_DIALOG_DATA) as ContainerEditDialogData;

  name = this.data.name;
  containerTypeId: number | null = this.data.containerTypeId;
  containerTypes = signal<ListContainerTypeQueryDto[]>([]);
  isLoadingTypes = signal(false);
  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.isLoadingTypes.set(true);

    const parentContainerId = this.data.parentContainerId;
    const parent$ = parentContainerId != null ? this.containerService.getById(parentContainerId) : of(null);

    forkJoin([this.productPageService.getContainerTypes(), parent$]).subscribe({
      next: ([typesResponse, parent]) => {
        const allTypes = typesResponse.items ?? [];
        // Same strict-smaller-than-parent rule as creating a container, but the
        // container's current type stays selectable even if it wouldn't otherwise
        // pass — so editing an existing container never hides its own value.
        const available = parent
          ? allTypes.filter(t => t.id === this.data.containerTypeId || (t.maxItems < parent.maxItems && t.maxContainers < parent.maxContainers))
          : allTypes;

        this.containerTypes.set(available);
        this.isLoadingTypes.set(false);
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

    this.containerService.update(this.data.id, {
      name: this.name.trim(),
      containerTypeId: this.containerTypeId,
    }).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(extractErrorMessage(err, 'Unable to save changes. Please try again.'));
      },
    });
  }
}
