import { Component, input, model } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { DynamicFieldConfig } from './dynamic-form.models';

// Renders a form from a field-config list instead of a hand-written template.
// A new field means adding an entry to a component's `fields` config, not a new
// template block — the same renderer is reused across every entity's dialog.
@Component({
  selector: 'app-dynamic-form',
  standalone: true,
  imports: [FormsModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  templateUrl: './dynamic-form.html',
  styleUrl: './dynamic-form.css',
})
export class DynamicForm {
  fields = input.required<DynamicFieldConfig[]>();
  formValue = model.required<Record<string, any>>();

  setValue(key: string, value: any): void {
    this.formValue.update(current => ({ ...current, [key]: value }));
  }
}
