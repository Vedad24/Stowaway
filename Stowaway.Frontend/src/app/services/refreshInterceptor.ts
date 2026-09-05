import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from './identity/auth/auth-service';
import { CurrentUserService } from './identity/auth/current-user-service';
import { API_CONFIG } from '../core/config/api-config';

// On a 401 (expired access token cookie), silently refresh via the cookie-driven
// refresh endpoint and retry the original request once. Skips /api/auth/* itself
// to avoid refresh-retry loops.
//
// Deliberately does NOT navigate to /login on refresh failure: this interceptor runs
// for every request, including the silent "am I logged in" probe (GET /User/me) fired
// on every app bootstrap for anonymous visitors on public pages - forcing a redirect
// there would kick a first-time visitor off the landing page. Route guards already own
// redirecting to /login (with a returnUrl) for routes that actually require auth.
export const refreshInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const currentUserService = inject(CurrentUserService);
  const config = inject(API_CONFIG);

  const isAuthRequest = req.url.includes(`/${config.auth.basePath}/`);

  return next(req).pipe(
    catchError((error) => {
      if (isAuthRequest || !(error instanceof HttpErrorResponse) || error.status !== 401) {
        return throwError(() => error);
      }

      return authService.refresh().pipe(
        switchMap((refreshed) => {
          if (!refreshed) {
            currentUserService.clearUser();
            return throwError(() => error);
          }
          return next(req);
        }),
        catchError(() => {
          currentUserService.clearUser();
          return throwError(() => error);
        }),
      );
    }),
  );
};
