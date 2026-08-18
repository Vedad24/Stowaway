import { Component, inject, OnInit, signal, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { DynamicForm } from '../../../../shared/dynamic-form/dynamic-form';
import {
  AutocompleteQuestion,
  DropdownQuestion,
  QuestionBase,
  TextboxQuestion,
} from '../../../../shared/dynamic-form/question-service/question.models';
import { UserService } from '../../../../services/identity/user/user-service';
import { ListUserQueryDto, RoleName } from '../../../../services/identity/user/user-service.models';

@Component({
  selector: 'app-employee-add-edit',
  imports: [DynamicForm, MatDialogModule, MatButtonModule],
  templateUrl: './employee-add-edit.html',
  styleUrl: './employee-add-edit.css',
})
export class EmployeeAddEdit{
  
  userService = inject(UserService);
  data = inject<ListUserQueryDto | null>(MAT_DIALOG_DATA, { optional: true });
  questions = signal<QuestionBase<string>[]>([]);
  isEdit = !!this.data;
  
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
        const questions = response.questions as QuestionBase<string>[];
        if (this.data) {
          this.prefillQuestions(questions, this.data);
        }
        this.questions.set([]);
        this.questions.set(questions);
      }
    )
  }

  private prefillQuestions(questions: QuestionBase<string>[], data: ListUserQueryDto): void {
    for (const question of questions) {
      if (question.key === 'role') {
        question.value = question.options?.find((opt) => opt.key === RoleName[data.roleId]) as any;
        continue;
      }
      if (question.key === 'password') {
        continue;
      }
      const value = (data as any)[question.key];
      if (value !== undefined) {
        question.value = value;
      }
    }
  }
  
  
  
}
