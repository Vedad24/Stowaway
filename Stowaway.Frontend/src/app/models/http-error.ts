import { HttpErrorResponse } from '@angular/common/http';

export function extractErrorMessage(err: unknown, fallback: string): string {
  if (err instanceof HttpErrorResponse) {
    const message = (err.error as { message?: string } | null)?.message;
    if (message) {
      return message;
    }
  }
  return fallback;
}
