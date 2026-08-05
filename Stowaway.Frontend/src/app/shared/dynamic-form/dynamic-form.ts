import { Component, computed, inject, input } from '@angular/core';
import { QuestionControlService } from './question-service/question-control-service';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { QuestionBase } from './question-service/question.models';
import { DynamicFormQuestion } from './dynamic-form-question/dynamic-form-question';

@Component({
  selector: 'app-dynamic-form',
  imports: [DynamicFormQuestion, ReactiveFormsModule],
  templateUrl: './dynamic-form.html',
  styleUrl: './dynamic-form.css',
})
export class DynamicForm {
  private readonly qcs = inject(QuestionControlService);

  readonly questions = input<QuestionBase<string>[] | null>([]);
  readonly form = computed<FormGroup>(() =>
    this.qcs.toFormGroup(this.questions() as QuestionBase<string>[]),
  );
  payLoad = '';

  //Call in component implementing dynamic form
  onSubmit() {
    this.payLoad = JSON.stringify(this.form().getRawValue());
    
  }
}
