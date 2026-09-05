import { IOptionsInfo } from '../../autocomplete-component/autocomplete-component';

export class QuestionBase<T> {
  value: T | undefined;
  key: string;
  label: string;
  required: boolean;
  order: number;
  controlType: string;
  type: string;
  options?: { key: string; value: string }[];
  optionInfo: IOptionsInfo;
  // when true, control renders disabled behind a "Change" button that unlocks it
  locked: boolean;

  constructor(
    options: {
      value?: T;
      key?: string;
      label?: string;
      required?: boolean;
      order?: number;
      controlType?: string;
      type?: string;
      options?: { key: string; value: string }[] ;
      optionInfo?: IOptionsInfo | null;
      locked?: boolean;
    } = {},
  ) {
    this.value = options.value;
    this.key = options.key || '';
    this.label = options.label || '';
    this.required = !!options.required;
    this.order = options.order === undefined ? 1 : options.order;
    this.controlType = options.controlType || '';
    this.type = options.type || '';
    this.options = options.options;
    this.optionInfo = options.optionInfo ?? { displayName: '' };
    this.locked = !!options.locked;
  }
}

export class TextboxQuestion extends QuestionBase<string> {
  override controlType = 'textbox';
}

export class DropdownQuestion extends QuestionBase<string> {
  override controlType = 'dropdown';
}

export class AutocompleteQuestion extends QuestionBase<string> {
  override controlType = 'autocomplete';
}

// value is a JSON-encoded array of selected tag ids, e.g. '[1,3,7]'
export class TagsQuestion extends QuestionBase<string> {
  override controlType = 'tags';
}

// value is a JSON-encoded array of base64 image strings (no data: prefix), e.g. '["/9j/4AAQ...","..."]'
export class ImagesQuestion extends QuestionBase<string> {
  override controlType = 'images';
}