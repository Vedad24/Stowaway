import { HttpInterceptorFn } from '@angular/common/http';

// Ensures the httpOnly auth cookies (and the XSRF-TOKEN cookie) are sent/received
// on every request to the backend, which runs on a different port in dev.
export const withCredentialsInterceptor: HttpInterceptorFn = (req, next) => {
    return next(req.clone({ withCredentials: true }));
};
