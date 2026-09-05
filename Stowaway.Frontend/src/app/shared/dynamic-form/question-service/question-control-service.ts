import { Injectable } from '@angular/core';
import { QuestionBase } from './question.models';
import { FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class QuestionControlService {
  toFormGroup(questions: QuestionBase<string>[]) {
    const group: any = {};

    questions.forEach((question) => {
      const validators: ValidatorFn[] = [];
      if (question.required) validators.push(Validators.required);
      if (question.type === 'email') validators.push(Validators.email);
      group[question.key] = new FormControl(question.value || '', validators);
    });
    return new FormGroup(group);
  }
}
