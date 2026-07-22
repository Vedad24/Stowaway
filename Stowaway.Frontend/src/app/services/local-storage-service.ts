import { isPlatformBrowser } from '@angular/common';
import { inject, Injectable, PLATFORM_ID } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LocalStorageService {
  private platformId = inject(PLATFORM_ID);
  
  getItem(key: string): string | null {
    if(isPlatformBrowser(this.platformId))
      return localStorage.getItem(key);
    return null;
  }
  
  setItem(key: string, value: string) {
    if(isPlatformBrowser(this.platformId))
      localStorage.setItem(key, value)
  }
  
}
