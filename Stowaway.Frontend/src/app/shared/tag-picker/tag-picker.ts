import { Component, OnInit, forwardRef, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TagApiService } from '../../services/storage/tag/tag';
import { TagDto } from '../../services/storage/tag/tag.model';
import { tagColor, tagTextColor } from '../tag-color';

@Component({
  selector: 'app-tag-picker',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatButtonModule, MatFormFieldModule, MatInputModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TagPicker),
      multi: true,
    },
  ],
  templateUrl: './tag-picker.html',
  styleUrl: './tag-picker.css',
})
export class TagPicker implements ControlValueAccessor, OnInit {
  private readonly tagService = inject(TagApiService);

  readonly allTags = signal<TagDto[]>([]);
  readonly selectedIds = signal<number[]>([]);
  readonly newTagNameControl = new FormControl('', { nonNullable: true });
  readonly isLoading = signal(false);
  readonly isCreating = signal(false);
  readonly errorMessage = signal<string | null>(null);
  private disabled = signal(false);

  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  ngOnInit(): void {
    this.isLoading.set(true);
    this.tagService.list().subscribe({
      next: (tags) => {
        this.allTags.set(tags);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load tags.');
        this.isLoading.set(false);
      },
    });
  }

  tagColor(id: number): string {
    return tagColor(id);
  }

  tagTextColor(id: number): string {
    return tagTextColor(tagColor(id));
  }

  isSelected(id: number): boolean {
    return this.selectedIds().includes(id);
  }

  toggleTag(id: number): void {
    if (this.disabled()) {
      return;
    }
    const current = this.selectedIds();
    const next = current.includes(id) ? current.filter((x) => x !== id) : [...current, id];
    this.selectedIds.set(next);
    this.emitChange();
  }

  addCustomTag(): void {
    const name = this.newTagNameControl.value.trim();
    if (!name || this.disabled()) {
      return;
    }

    this.isCreating.set(true);
    this.tagService.create({ name }).subscribe({
      next: (tag) => {
        this.isCreating.set(false);
        this.newTagNameControl.setValue('');
        if (!this.allTags().some((t) => t.id === tag.id)) {
          this.allTags.update((tags) => [...tags, tag].sort((a, b) => a.name.localeCompare(b.name)));
        }
        if (!this.selectedIds().includes(tag.id)) {
          this.selectedIds.update((ids) => [...ids, tag.id]);
          this.emitChange();
        }
      },
      error: () => {
        this.isCreating.set(false);
        this.errorMessage.set('Unable to create tag.');
      },
    });
  }

  private emitChange(): void {
    this.onTouched();
    this.onChange(JSON.stringify(this.selectedIds()));
  }

  /* ===== ControlValueAccessor ===== */

  writeValue(value: string): void {
    try {
      const parsed = value ? JSON.parse(value) : [];
      this.selectedIds.set(Array.isArray(parsed) ? parsed : []);
    } catch {
      this.selectedIds.set([]);
    }
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }
}
