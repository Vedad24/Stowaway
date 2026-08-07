import { Component, inject, OnInit, signal, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { DynamicForm } from '../../../../shared/dynamic-form/dynamic-form';
import {
  AutocompleteQuestion,
  DropdownQuestion,
  QuestionBase,
  TextboxQuestion,
} from '../../../../shared/dynamic-form/question-service/question.models';
import { UserService } from '../../../../services/identity/user/user-service';

@Component({
  selector: 'app-employee-add-edit',
  imports: [DynamicForm, MatDialogModule, MatButtonModule],
  templateUrl: './employee-add-edit.html',
  styleUrl: './employee-add-edit.css',
})
export class EmployeeAddEdit{
  
  userService = inject(UserService);
  questions = signal<QuestionBase<string>[]>([]);
  
  // dummy = [
  //   new TextboxQuestion({
  //     key: 'firstName',
  //     label: 'First Name',
  //     required: true,
  //     type: 'text',
  //   }),
  //   new TextboxQuestion({
  //     key: 'lastName',
  //     label: 'Last Name',
  //     required: true,
  //     type: 'text',
  //   }),
  //   new DropdownQuestion({
  //     key: 'occupation',
  //     label: 'Occupation',
  //     required: true,
  //     options: [
  //       { key: 'janitor', value: 'Janitor' },
  //       { key: 'programmer', value: 'Programmer' },
  //     ],
  //   }),
  //   new AutocompleteQuestion({
  //     key: 'role',
  //     label: 'Role',
  //     required: true,
  //     options: [
  //       { key: 'manager', value: 'Manager' },
  //       { key: 'admin', value: 'Admin' },
  //       { key: 'user', value: 'User' },
  //     ],
  //     optionInfo: { displayName: 'value' },
  //   }),
  // ];

  constructor() {
    
    this.userService.getEmployeeForm().subscribe
    (
      (response)=>
      {
        console.log("form:", response);
        this.questions.set([]);
        this.questions.set(response.questions as QuestionBase<string>[]);
      }
    )
  }
  
  
  
}
