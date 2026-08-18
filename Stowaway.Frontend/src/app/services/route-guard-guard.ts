import { ActivatedRoute, CanActivateFn, Router } from '@angular/router';
import { AuthService } from './identity/auth/auth-service';
import { Inject, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export const routeGuardGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const platformId = inject(PLATFORM_ID);

  //allows manually typing in URLs after authenticating
  if(!isPlatformBrowser(platformId))
    return true;
  
  if( authService.isLoggedIn())
    return true;
  return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
};
