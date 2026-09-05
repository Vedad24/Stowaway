import { Component, input, OnInit } from '@angular/core';
import { FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { AutocompleteComponent } from '../../autocomplete-component/autocomplete-component';
import { TagPicker } from '../../tag-picker/tag-picker';
import { ImagePicker } from '../../image-picker/image-picker';
import { QuestionBase } from '../question-service/question.models';
import { MatButtonModule } from '@angular/material/button';

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
    MatButtonModule
  ],
  templateUrl: './dynamic-form-question.html',
  styleUrl: './dynamic-form-question.css',
})
export class DynamicFormQuestion implements OnInit {
  readonly question = input.required<QuestionBase<string>>();
  readonly form = input.required<FormGroup>();

  isLocked = false;

  ngOnInit(): void {
    this.isLocked = !!this.question().locked;
  }

  get isValid() {
    return this.form().controls[this.question().key].valid;
  }

  unlock(): void {
    const control = this.form().controls[this.question().key];
    control.enable();
    control.setValidators([Validators.required, Validators.minLength(8)]);
    control.updateValueAndValidity();
    this.isLocked = false;
  }
}
