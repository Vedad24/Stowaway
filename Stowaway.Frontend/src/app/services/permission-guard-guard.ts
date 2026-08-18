import { CanActivateFn, Router } from '@angular/router';
import { CurrentUserService } from './identity/auth/current-user-service';
import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export const permissionGuard = (permission: string): CanActivateFn => (route, state) => {
  const currentUserService = inject(CurrentUserService);
  const router = inject(Router);
  const platformId = inject(PLATFORM_ID);

  //allows manually typing in URLs after authenticating
  if (!isPlatformBrowser(platformId))
    return true;

  if (currentUserService.permissions.includes(permission))
    return true;

  //not logged in at all - let the login flow handle it, not a permission denial
  if (currentUserService.currentUser === null)
    return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });

  //logged in but missing the permission: bounce back to wherever they came from
  //(e.g. clicking a button that should've been hidden) instead of a forbidden page
  const previousUrl = router.url;
  if (previousUrl && previousUrl !== state.url && previousUrl !== '/') {
    return router.createUrlTree([previousUrl]);
  }
  return router.createUrlTree(['/choose-module']);
};
