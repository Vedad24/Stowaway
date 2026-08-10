export type DynamicFieldType = 'text' | 'number' | 'textarea' | 'select';

export interface DynamicSelectOption {
  value: number | string;
  label: string;
}

export interface DynamicFieldConfig {
  key: string;
  label: string;
  type: DynamicFieldType;
  required?: boolean;
  disabled?: boolean;
  min?: number;
  rows?: number;
  options?: DynamicSelectOption[];
}
