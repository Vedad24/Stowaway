import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from './identity/auth/auth-service';
import { CurrentUserService } from './identity/auth/current-user-service';
import { ApiEndpoints } from '../shared/constants/api-endpoints';

// On a 401 (expired access token cookie), silently refresh via the cookie-driven
// refresh endpoint and retry the original request once. Skips /api/auth/* itself
// to avoid refresh-retry loops.
export const refreshInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const currentUserService = inject(CurrentUserService);
  const router = inject(Router);

  const isAuthRequest = req.url.includes(`/${ApiEndpoints.Auth}/`);

  return next(req).pipe(
    catchError((error) => {
      if (isAuthRequest || !(error instanceof HttpErrorResponse) || error.status !== 401) {
        return throwError(() => error);
      }

      return authService.refresh().pipe(
        switchMap((refreshed) => {
          if (!refreshed) {
            currentUserService.clearUser();
            router.navigate(['/login']);
            return throwError(() => error);
          }
          return next(req);
        }),
        catchError(() => {
          currentUserService.clearUser();
          router.navigate(['/login']);
          return throwError(() => error);
        }),
      );
    }),
  );
};
