import { Component, input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { AutocompleteComponent } from '../../autocomplete-component/autocomplete-component';
import { TagPicker } from '../../tag-picker/tag-picker';
import { ImagePicker } from '../../image-picker/image-picker';
import { QuestionBase } from '../question-service/question.models';

@Component({
  selector: 'app-dynamic-form-question',
  imports: [
    ReactiveFormsModule,
    AutocompleteComponent,
    TagPicker,
    ImagePicker,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
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
