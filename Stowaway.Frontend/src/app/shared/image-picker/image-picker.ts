import { Component, computed, forwardRef, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { ImageCroppedEvent, ImageCropperComponent } from 'ngx-image-cropper';

type QualityPreset = 'balanced' | 'low';

const QUALITY_PRESETS: Record<QualityPreset, { quality: number; maxDimension: number; label: string }> = {
  balanced: { quality: 75, maxDimension: 1280, label: 'Balanced' },
  low: { quality: 50, maxDimension: 900, label: 'Low' },
};

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

  readonly presetKeys: QualityPreset[] = ['balanced', 'low'];
  readonly presets = QUALITY_PRESETS;
  readonly qualityPreset = signal<QualityPreset>('balanced');
  readonly currentPreset = computed(() => QUALITY_PRESETS[this.qualityPreset()]);
  readonly livePreviewBytes = signal<number | null>(null);
  readonly livePreviewSrc = signal<string | null>(null);

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

  setQualityPreset(preset: QualityPreset, cropper: ImageCropperComponent): void {
    this.qualityPreset.set(preset);
    // cropper.crop() reads from its internal `state.options`, not from the raw
    // [imageQuality]/[resizeToWidth]/[resizeToHeight] @Inputs directly — those only reach
    // `state.options` via ngOnChanges, which runs on Angular's next change-detection pass,
    // not synchronously here. Without this, crop() would use last click's settings, so the
    // preview would always lag one click behind. Write straight into `state.options` (not
    // part of the public API, hence the cast) so the very next crop() call already sees it.
    const p = QUALITY_PRESETS[preset];
    (cropper as unknown as { state: { setOptions(o: Record<string, unknown>): void } }).state.setOptions({
      imageQuality: p.quality,
      resizeToWidth: p.maxDimension,
      resizeToHeight: p.maxDimension,
    });
    this.applyCropResult(cropper.crop());
  }

  onImageCropped(event: ImageCroppedEvent): void {
    this.applyCropResult(event);
  }

  private applyCropResult(result: ImageCroppedEvent | null): void {
    if (result?.base64) {
      this.livePreviewSrc.set(result.base64);
      this.livePreviewBytes.set(this.estimateBytes(result.base64));
    }
  }

  estimateBytes(base64: string): number {
    const raw = base64.includes(',') ? base64.split(',')[1] : base64;
    return Math.round((raw.length * 3) / 4);
  }

  formatBytes(bytes: number): string {
    return bytes < 1024 ? `${bytes} B` : `${Math.round(bytes / 1024)} KB`;
  }

  removeImage(index: number): void {
    if (this.disabled()) {
      return;
    }
    this.images.update((imgs) => imgs.filter((_, i) => i !== index));
    this.emitChange();
  }

  private advanceQueue(): void {
    this.livePreviewBytes.set(null);
    this.livePreviewSrc.set(null);
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
