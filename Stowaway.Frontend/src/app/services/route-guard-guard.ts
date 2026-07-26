import { ActivatedRoute, CanActivateFn, Router } from '@angular/router';
import { AuthService } from './identity/auth/auth-service';
import { inject } from '@angular/core';

export const routeGuardGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  if( authService.isLoggedIn())
    return true;
  return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
};
