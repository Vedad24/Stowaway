import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface ImageLightboxData {
  images: string[];
  startIndex: number;
}

const MIN_ZOOM = 1;
const MAX_ZOOM = 4;
const ZOOM_STEP = 0.5;

@Component({
  selector: 'app-image-lightbox',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './image-lightbox.html',
  styleUrl: './image-lightbox.css',
})
export class ImageLightbox {
  readonly dialogRef = inject(MatDialogRef<ImageLightbox>);
  readonly data = inject(MAT_DIALOG_DATA) as ImageLightboxData;

  readonly index = signal(this.data.startIndex ?? 0);
  readonly zoom = signal(1);
  readonly pan = signal({ x: 0, y: 0 });

  private dragging = false;
  private dragStart = { x: 0, y: 0 };
  private panStart = { x: 0, y: 0 };

  readonly currentSrc = computed(() => this.data.images[this.index()]);
  readonly zoomPercent = computed(() => Math.round(this.zoom() * 100));

  next(): void {
    if (this.zoom() > 1) {
      return;
    }
    const n = this.data.images.length;
    this.index.set((this.index() + 1) % n);
  }

  prev(): void {
    if (this.zoom() > 1) {
      return;
    }
    const n = this.data.images.length;
    this.index.set((this.index() - 1 + n) % n);
  }

  goTo(i: number): void {
    this.resetZoom();
    this.index.set(i);
  }

  zoomIn(): void {
    this.setZoom(this.zoom() + ZOOM_STEP);
  }

  zoomOut(): void {
    this.setZoom(this.zoom() - ZOOM_STEP);
  }

  resetZoom(): void {
    this.zoom.set(MIN_ZOOM);
    this.pan.set({ x: 0, y: 0 });
  }

  toggleZoom(): void {
    if (this.zoom() > 1) {
      this.resetZoom();
    } else {
      this.setZoom(2);
    }
  }

  onWheel(event: WheelEvent): void {
    event.preventDefault();
    this.setZoom(this.zoom() + (event.deltaY < 0 ? ZOOM_STEP : -ZOOM_STEP));
  }

  onPointerDown(event: PointerEvent): void {
    if (this.zoom() <= 1) {
      return;
    }
    this.dragging = true;
    this.dragStart = { x: event.clientX, y: event.clientY };
    this.panStart = this.pan();
    (event.currentTarget as HTMLElement).setPointerCapture(event.pointerId);
  }

  onPointerMove(event: PointerEvent): void {
    if (!this.dragging) {
      return;
    }
    this.pan.set({
      x: this.panStart.x + (event.clientX - this.dragStart.x),
      y: this.panStart.y + (event.clientY - this.dragStart.y),
    });
  }

  onPointerUp(): void {
    this.dragging = false;
  }

  onKeydown(event: KeyboardEvent): void {
    if (event.key === 'ArrowRight') {
      this.next();
    } else if (event.key === 'ArrowLeft') {
      this.prev();
    } else if (event.key === 'Escape') {
      this.close();
    } else if (event.key === '+' || event.key === '=') {
      this.zoomIn();
    } else if (event.key === '-') {
      this.zoomOut();
    }
  }

  close(): void {
    this.dialogRef.close();
  }

  private setZoom(z: number): void {
    const clamped = Math.min(MAX_ZOOM, Math.max(MIN_ZOOM, z));
    this.zoom.set(clamped);
    if (clamped === MIN_ZOOM) {
      this.pan.set({ x: 0, y: 0 });
    }
  }
}
