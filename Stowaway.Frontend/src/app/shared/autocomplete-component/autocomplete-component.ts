import { Component, computed, ElementRef, forwardRef, input, Input, provideAppInitializer, signal, ViewChild } from '@angular/core';
import { ControlValueAccessor, FormControl, FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { MatFormField, MatInputModule, MatLabel } from "@angular/material/input";
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatAutocompleteModule} from '@angular/material/autocomplete';

@Component({
  selector: 'app-autocomplete-component',
  imports: [MatFormFieldModule, MatLabel, MatAutocompleteModule, FormsModule, MatInputModule, ReactiveFormsModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AutocompleteComponent),
      multi: true
    }
  ],
  templateUrl: './autocomplete-component.html',
  styleUrl: './autocomplete-component.css',
  standalone: true
})
export class AutocompleteComponent implements ControlValueAccessor {

  //Inputs from parent
  options = input.required<any[]>();
  optionInfo = input.required<IOptionsInfo>();

  //Component specific
  // internal state
  protected value = signal<any | null>(null);
  private filterText = signal('');
  private disabled = signal(false);
  
  filteredOptions = computed(() => {
    const filterValue = this.filterText().toLowerCase();
    return this.options().filter(o => o[this.optionInfo().displayName].toLowerCase().includes(filterValue));
  });
  
  displayFn = (value: any) :string => {
    if(!value) return '';
    const key = this.optionInfo().displayName;
    return String(value[key] ?? '');
  }

  // filter(): void {
  //   queueMicrotask(() => {});
  // }

  /* ===== CVA API ===== */

  private onChange = (_: any) => {};
  public onTouched = () => {};

  writeValue(value: any): void {
    this.value.set(value);
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  /* ===== UI handlers ===== */

  onInput(value: string) {
    this.filterText.set(value);
  }

  onSelect(option: any) {
    this.value.set(option);
    this.onChange(option);
    this.onTouched();
  }
}

export interface IOptionsInfo {
  displayName : string
}