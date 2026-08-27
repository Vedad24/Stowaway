import { Component, OnInit, inject, signal } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { ContainerApiService } from '../../../services/storage/container/container';
import { ProductPageService } from '../../../services/sales/product-page/product-page-service';
import { DynamicForm } from '../../../shared/dynamic-form/dynamic-form';
import { DropdownQuestion, QuestionBase, TextboxQuestion } from '../../../shared/dynamic-form/question-service/question.models';
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
  imports: [MatDialogModule, MatIconModule, DynamicForm],
  templateUrl: './container-edit.html',
  styleUrl: './container-edit.css',
})
export class ContainerEdit implements OnInit {
  readonly dialogRef = inject(MatDialogRef<ContainerEdit>);
  private readonly containerService = inject(ContainerApiService);
  private readonly productPageService = inject(ProductPageService);
  private readonly data = inject(MAT_DIALOG_DATA) as ContainerEditDialogData;

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
        const available = parent
          ? allTypes.filter(t => t.id === this.data.containerTypeId || (t.maxItems < parent.maxItems && t.maxContainers < parent.maxContainers))
          : allTypes;

        this.isLoadingTypes.set(false);

        this.questions.set([
          new TextboxQuestion({ key: 'name', label: 'Name', required: true, type: 'text', order: 1, value: this.data.name }),
          new DropdownQuestion({
            key: 'containerTypeId',
            label: 'Container type',
            required: true,
            order: 2,
            value: String(this.data.containerTypeId),
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
