import { Component, input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { AutocompleteComponent } from '../../autocomplete-component/autocomplete-component';
import { QuestionBase } from '../question-service/question.models';

@Component({
  selector: 'app-dynamic-form-question',
  imports: [ReactiveFormsModule, AutocompleteComponent],
  templateUrl: './dynamic-form-question.html',
  styleUrl: './dynamic-form-question.css',
})
export class DynamicFormQuestion {
  readonly question = input.required<QuestionBase<string>>();
  readonly form = input.required<FormGroup>();
  
  get isValid() {
    return this.form().controls[this.question().key].valid;
  }
}
