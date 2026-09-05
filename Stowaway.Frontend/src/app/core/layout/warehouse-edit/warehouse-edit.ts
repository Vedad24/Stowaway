import { Component, OnInit, inject, signal } from '@angular/core';
import { MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { DynamicForm } from '../../../shared/dynamic-form/dynamic-form';
import { QuestionBase, TextboxQuestion } from '../../../shared/dynamic-form/question-service/question.models';

export interface WarehouseEditDialogData {
  id: number;
  name: string;
  description: string;
  city: string;
  address: string;
  capacity: number;
  isEnabled: boolean;
}

@Component({
  selector: 'app-warehouse-edit',
  standalone: true,
  imports: [MatDialogModule, MatIconModule, DynamicForm],
  templateUrl: './warehouse-edit.html',
  styleUrl: './warehouse-edit.css',
})
export class WarehouseEdit implements OnInit {
  private readonly data = inject(MAT_DIALOG_DATA) as WarehouseEditDialogData;

  questions = signal<QuestionBase<string>[]>([]);

  ngOnInit(): void {
    this.questions.set([
      new TextboxQuestion({ key: 'name', label: 'Name', required: true, type: 'text', order: 1, value: this.data.name }),
      new TextboxQuestion({ key: 'description', label: 'Description', type: 'text', order: 2, value: this.data.description }),
      new TextboxQuestion({ key: 'city', label: 'City', type: 'text', order: 3, value: this.data.city }),
      new TextboxQuestion({ key: 'address', label: 'Address', type: 'text', order: 4, value: this.data.address }),
      new TextboxQuestion({ key: 'capacity', label: 'Capacity', required: true, type: 'number', value: String(this.data.capacity), order: 5 }),
    ]);
  }
}
