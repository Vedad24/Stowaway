import { HttpInterceptorFn } from '@angular/common/http';
import { CurrentUserService } from './identity/auth/current-user-service';
import { inject } from '@angular/core';

export const httpAuthInterceptor: HttpInterceptorFn = (req, next) => {
    // Make sure 'jwt_token' matches the exact key you use when saving the token
    const currentUserService = inject(CurrentUserService);
    const token = currentUserService.currentUser?.accessToken;
    if (token) {
    const clonedRequest = req.clone({
        setHeaders: {
        Authorization: `Bearer ${token}`
        }
    });
    return next(clonedRequest);
    }

    return next(req);
};