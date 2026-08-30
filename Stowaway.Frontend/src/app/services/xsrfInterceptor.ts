import { HttpInterceptorFn } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { inject, PLATFORM_ID } from '@angular/core';

const XSRF_COOKIE_NAME = 'XSRF-TOKEN';
const XSRF_HEADER_NAME = 'X-XSRF-TOKEN';

function readCookie(name: string): string | null {
  const match = document.cookie.match(new RegExp('(?:^|; )' + name + '=([^;]*)'));
  return match ? decodeURIComponent(match[1]) : null;
}

// Angular's built-in withXsrfConfiguration() interceptor only attaches the header to relative
// URLs - it explicitly skips any absolute URL (http://.../...), which is all this app ever sends,
// since the backend is a different origin. So it's replaced with this: read the readable XSRF-TOKEN
// cookie ourselves and echo it back as X-XSRF-TOKEN on mutating requests.
export const xsrfInterceptor: HttpInterceptorFn = (req, next) => {
  const platformId = inject(PLATFORM_ID);

  if (!isPlatformBrowser(platformId) || req.method === 'GET' || req.method === 'HEAD') {
    return next(req);
  }

  const token = readCookie(XSRF_COOKIE_NAME);
  if (!token) {
    return next(req);
  }

  return next(req.clone({ setHeaders: { [XSRF_HEADER_NAME]: token } }));
};
