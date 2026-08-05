import { Component } from '@angular/core';
import { DynamicForm } from '../../../../shared/dynamic-form/dynamic-form';
import {
  AutocompleteQuestion,
  DropdownQuestion,
  TextboxQuestion,
} from '../../../../shared/dynamic-form/question-service/question.models';

@Component({
  selector: 'app-employee-add-edit',
  imports: [DynamicForm],
  templateUrl: './employee-add-edit.html',
  styleUrl: './employee-add-edit.css',
})
export class EmployeeAddEdit {
  readonly questions = [
    new TextboxQuestion({
      key: 'firstName',
      label: 'First Name',
      required: true,
      type: 'text',
    }),
    new TextboxQuestion({
      key: 'lastName',
      label: 'Last Name',
      required: true,
      type: 'text',
    }),
    new DropdownQuestion({
      key: 'occupation',
      label: 'Occupation',
      required: true,
      options: [
        { key: 'janitor', value: 'Janitor' },
        { key: 'programmer', value: 'Programmer' },
      ],
    }),
    new AutocompleteQuestion({
      key: 'role',
      label: 'Role',
      required: true,
      options: [
        { key: 'manager', value: 'Manager' },
        { key: 'admin', value: 'Admin' },
        { key: 'user', value: 'User' },
      ],
      optionInfo: { displayName: 'value' },
    }),
  ];
}
