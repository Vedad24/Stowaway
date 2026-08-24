import { Component, forwardRef, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { ImageCropperComponent } from 'ngx-image-cropper';

@Component({
  selector: 'app-image-picker',
  standalone: true,
  imports: [CommonModule, MatButtonModule, ImageCropperComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ImagePicker),
      multi: true,
    },
  ],
  templateUrl: './image-picker.html',
  styleUrl: './image-picker.css',
})
export class ImagePicker implements ControlValueAccessor {
  readonly images = signal<string[]>([]);
  readonly cropQueue = signal<File[]>([]);
  readonly croppingFile = signal<File | null>(null);
  private disabled = signal(false);

  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  onFilesSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = input.files ? Array.from(input.files) : [];
    input.value = '';
    if (files.length === 0 || this.disabled()) {
      return;
    }
    this.cropQueue.update((q) => [...q, ...files]);
    if (!this.croppingFile()) {
      this.advanceQueue();
    }
  }

  confirmCrop(cropper: ImageCropperComponent): void {
    const result = cropper.crop();
    const base64 = result?.base64;
    if (base64) {
      const stripped = base64.split(',')[1] ?? base64;
      this.images.update((imgs) => [...imgs, stripped]);
      this.emitChange();
    }
    this.advanceQueue();
  }

  cancelCrop(): void {
    this.advanceQueue();
  }

  removeImage(index: number): void {
    if (this.disabled()) {
      return;
    }
    this.images.update((imgs) => imgs.filter((_, i) => i !== index));
    this.emitChange();
  }

  private advanceQueue(): void {
    const queue = this.cropQueue();
    if (queue.length > 0) {
      this.croppingFile.set(queue[0]);
      this.cropQueue.set(queue.slice(1));
    } else {
      this.croppingFile.set(null);
    }
  }

  private emitChange(): void {
    this.onTouched();
    this.onChange(JSON.stringify(this.images()));
  }

  /* ===== ControlValueAccessor ===== */

  writeValue(value: string): void {
    try {
      const parsed = value ? JSON.parse(value) : [];
      this.images.set(Array.isArray(parsed) ? parsed : []);
    } catch {
      this.images.set([]);
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
