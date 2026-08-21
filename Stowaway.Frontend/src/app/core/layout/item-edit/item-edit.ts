import { Component, OnInit, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { ContainerTreeService } from '../../../services/storage/container/container-tree';
import { SupplierApiService } from '../../../services/storage/supplier/supplier';
import { DynamicForm } from '../../../shared/dynamic-form/dynamic-form';
import { DropdownQuestion, QuestionBase, TagsQuestion, TextboxQuestion } from '../../../shared/dynamic-form/question-service/question.models';
import { extractErrorMessage } from '../../../models/http-error';

export interface ItemEditDialogData {
  id: number;
  name: string;
  description: string;
  quantity: number;
  supplierId: number;
  containerId: number;
  warehouseId: number;
  tagIds: number[];
}

@Component({
  selector: 'app-item-edit',
  standalone: true,
  imports: [MatDialogModule, MatIconModule, DynamicForm],
  templateUrl: './item-edit.html',
  styleUrl: './item-edit.css',
})
export class ItemEdit implements OnInit {
  readonly dialogRef = inject(MatDialogRef<ItemEdit>);
  private readonly containerTree = inject(ContainerTreeService);
  private readonly supplierService = inject(SupplierApiService);
  private readonly data = inject(MAT_DIALOG_DATA) as ItemEditDialogData;

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

      this.questions.set([
        new TextboxQuestion({ key: 'name', label: 'Name', required: true, type: 'text', order: 1, value: this.data.name }),
        new TextboxQuestion({ key: 'description', label: 'Description', type: 'text', order: 2, value: this.data.description }),
        new TextboxQuestion({ key: 'quantity', label: 'Quantity', required: true, type: 'number', order: 3, value: String(this.data.quantity) }),
        new DropdownQuestion({
          key: 'supplierId',
          label: 'Supplier',
          required: true,
          order: 4,
          value: String(this.data.supplierId),
          options: suppliers.map(s => ({ key: String(s.id), value: s.name })),
        }),
        new DropdownQuestion({
          key: 'containerId',
          label: 'Place (container)',
          required: true,
          order: 5,
          value: String(this.data.containerId),
          options: containerOptions.map(c => ({ key: String(c.id), value: c.label })),
        }),
        new TagsQuestion({ key: 'tagIds', label: 'Tags', order: 6, value: JSON.stringify(this.data.tagIds ?? []) }),
      ]);
      this.isLoadingOptions.set(false);
    }).catch((err) => {
      this.isLoadingOptions.set(false);
      this.errorMessage.set(extractErrorMessage(err, 'Unable to load containers or suppliers.'));
    });
  }
}
