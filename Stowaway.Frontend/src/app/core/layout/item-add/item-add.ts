import { Component, OnInit, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ContainerTreeService } from '../../../services/storage/container/container-tree';
import { SupplierApiService } from '../../../services/storage/supplier/supplier';
import { DynamicForm } from '../../../shared/dynamic-form/dynamic-form';
import { DropdownQuestion, QuestionBase, TagsQuestion, TextboxQuestion } from '../../../shared/dynamic-form/question-service/question.models';
import { extractErrorMessage } from '../../../models/http-error';

export interface ItemAddDialogData {
  warehouseId: number;
  containerId: number | null;
}

@Component({
  selector: 'app-item-add',
  standalone: true,
  imports: [MatDialogModule, DynamicForm],
  templateUrl: './item-add.html',
  styleUrl: './item-add.css',
})
export class ItemAdd implements OnInit {
  readonly dialogRef = inject(MatDialogRef<ItemAdd>);
  private readonly containerTree = inject(ContainerTreeService);
  private readonly supplierService = inject(SupplierApiService);
  private readonly data = inject(MAT_DIALOG_DATA) as ItemAddDialogData;

  questions = signal<QuestionBase<string>[]>([]);
  isLoadingOptions = signal(false);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.isLoadingOptions.set(true);
    Promise.all([
      this.containerTree.loadOptions(this.data.warehouseId),
      firstValueFrom(this.supplierService.list()),
    ]).then(([containerOptions, supplierResponse]) => {
      const suppliers = supplierResponse.items ?? [];
      const defaultContainerId = this.data.containerId ?? containerOptions[0]?.id ?? null;

      this.questions.set([
        new TextboxQuestion({ key: 'name', label: 'Name', required: true, type: 'text', order: 1 }),
        new TextboxQuestion({ key: 'description', label: 'Description', type: 'text', order: 2 }),
        new TextboxQuestion({ key: 'quantity', label: 'Quantity', required: true, type: 'number', value: '1', order: 3 }),
        new DropdownQuestion({
          key: 'supplierId',
          label: 'Supplier',
          required: true,
          order: 4,
          options: suppliers.map(s => ({ key: String(s.id), value: s.name })),
        }),
        new DropdownQuestion({
          key: 'containerId',
          label: 'Place (container)',
          required: true,
          order: 5,
          value: defaultContainerId != null ? String(defaultContainerId) : undefined,
          options: containerOptions.map(c => ({ key: String(c.id), value: c.label })),
        }),
        new TagsQuestion({ key: 'tagIds', label: 'Tags', order: 6, value: '[]' }),
      ]);
      this.isLoadingOptions.set(false);
    }).catch((err) => {
      this.isLoadingOptions.set(false);
      this.errorMessage.set(extractErrorMessage(err, 'Unable to load containers or suppliers.'));
    });
  }
}
