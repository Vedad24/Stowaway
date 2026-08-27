import { Component, OnInit, signal } from '@angular/core';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { DynamicForm } from '../../../shared/dynamic-form/dynamic-form';
import { QuestionBase, TextboxQuestion } from '../../../shared/dynamic-form/question-service/question.models';

@Component({
  selector: 'app-warehouse-add',
  standalone: true,
  imports: [MatDialogModule, MatIconModule, DynamicForm],
  templateUrl: './warehouse-add.html',
  styleUrl: './warehouse-add.css',
})
export class WarehouseAdd implements OnInit {
  questions = signal<QuestionBase<string>[]>([]);

  ngOnInit(): void {
    this.questions.set([
      new TextboxQuestion({ key: 'name', label: 'Name', required: true, type: 'text', order: 1 }),
      new TextboxQuestion({ key: 'description', label: 'Description', type: 'text', order: 2 }),
      new TextboxQuestion({ key: 'city', label: 'City', type: 'text', order: 3 }),
      new TextboxQuestion({ key: 'address', label: 'Address', type: 'text', order: 4 }),
      new TextboxQuestion({ key: 'capacity', label: 'Capacity', required: true, type: 'number', value: '1', order: 5 }),
    ]);
  }
}
