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
      if (question.type === 'password') validators.push(Validators.minLength(8));
      group[question.key] = new FormControl(
        { value: question.value || '', disabled: !!question.locked },
        validators,
      );
    });
    return new FormGroup(group);
  }
}
