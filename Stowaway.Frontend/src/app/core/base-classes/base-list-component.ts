// src/app/core/components/base-list.component.ts

import { signal } from '@angular/core';
import {BaseComponent} from './base-component';

export abstract class BaseListComponent<TDto> extends BaseComponent{
  items = signal<TDto[]>([]);

  /**
   * Konkretnu implementaciju punjenja podataka ostavljamo djeci.
   */
  protected abstract loadData(): void;

  /**
   * Helper koji možeš zvati iz ngOnInit dječije komponente.
   */
  protected initList(): void {
    this.loadData();
  }
}
