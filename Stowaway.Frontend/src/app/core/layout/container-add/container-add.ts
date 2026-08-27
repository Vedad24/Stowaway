import { Component, OnInit, inject, signal } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { ContainerApiService } from '../../../services/storage/container/container';
import { ProductPageService } from '../../../services/sales/product-page/product-page-service';
import { DynamicForm } from '../../../shared/dynamic-form/dynamic-form';
import { DropdownQuestion, QuestionBase, TextboxQuestion } from '../../../shared/dynamic-form/question-service/question.models';
import { extractErrorMessage } from '../../../models/http-error';

export interface ContainerAddDialogData {
  warehouseId: number;
  parentContainerId: number | null;
}

@Component({
  selector: 'app-container-add',
  standalone: true,
  imports: [MatDialogModule, MatIconModule, DynamicForm],
  templateUrl: './container-add.html',
  styleUrl: './container-add.css',
})
export class ContainerAdd implements OnInit {
  readonly dialogRef = inject(MatDialogRef<ContainerAdd>);
  private readonly containerService = inject(ContainerApiService);
  private readonly productPageService = inject(ProductPageService);
  private readonly data = inject(MAT_DIALOG_DATA) as ContainerAddDialogData;

  questions = signal<QuestionBase<string>[]>([]);
  isLoadingTypes = signal(false);
  errorMessage = signal<string | null>(null);

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

        this.isLoadingTypes.set(false);

        if (parent && available.length === 0) {
          this.errorMessage.set('No container type is small enough to fit inside this container.');
          return;
        }

        this.questions.set([
          new TextboxQuestion({ key: 'name', label: 'Name', required: true, type: 'text', order: 1 }),
          new DropdownQuestion({
            key: 'containerTypeId',
            label: 'Container type',
            required: true,
            order: 2,
            options: available.map(t => ({ key: String(t.id), value: t.displayName })),
          }),
        ]);
      },
      error: (err) => {
        this.isLoadingTypes.set(false);
        this.errorMessage.set(extractErrorMessage(err, 'Unable to load container types.'));
      },
    });
  }
}
