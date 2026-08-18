// src/app/core/components/base-list.component.ts

import { signal } from '@angular/core';
import {BaseComponent} from './base-component';

export abstract class BaseListComponent<TDto> extends BaseComponent{
  items = signal<TDto[]>([]);

  /**
   * The concrete data-loading implementation is left to subclasses.
   */
  protected abstract loadData(): void;

  /**
   * Helper you can call from a child component's ngOnInit.
   */
  protected initList(): void {
    this.loadData();
  }
}
