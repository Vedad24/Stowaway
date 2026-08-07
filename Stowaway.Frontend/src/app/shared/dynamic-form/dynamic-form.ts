import { Component, computed, inject, input } from '@angular/core';
import { QuestionControlService } from './question-service/question-control-service';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { QuestionBase } from './question-service/question.models';
import { DynamicFormQuestion } from './dynamic-form-question/dynamic-form-question';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-dynamic-form',
  imports: [DynamicFormQuestion, ReactiveFormsModule, MatButtonModule],
  templateUrl: './dynamic-form.html',
  styleUrl: './dynamic-form.css',
})
export class DynamicForm {
  private readonly qcs = inject(QuestionControlService);
  readonly dialogRef = inject(MatDialogRef<DynamicForm>);
  readonly questions = input<QuestionBase<string>[] | null>([]);
  readonly form = computed<FormGroup>(() =>
    this.qcs.toFormGroup(this.questions() as QuestionBase<string>[]),
  );
  payLoad = '';
  
  
  onSubmit() {
    if(!this.form().invalid)
    {
      //get form value
      this.payLoad = JSON.stringify(this.form().getRawValue());
      
      //close dialog while emitting payload <- works like throwing exception all the way to whoever subscribed to afterClosed()
      this.dialogRef.close(this.payLoad);
    }
    //console.log("payload:", this.payLoad);
  }
  onCancel() {
    this.dialogRef.close(false);
  }


}
