import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors, withXsrfConfiguration } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { withCredentialsInterceptor } from './services/withCredentialsInterceptor';
import { refreshInterceptor } from './services/refreshInterceptor';
import { ThemeService } from './services/theme-service';
import { CurrentUserService } from './services/identity/auth/current-user-service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideClientHydration(withEventReplay()),
    provideHttpClient(
      withFetch(),
      withXsrfConfiguration({ cookieName: 'XSRF-TOKEN', headerName: 'X-XSRF-TOKEN' }),
      withInterceptors([withCredentialsInterceptor, refreshInterceptor]),
    ),
    provideAppInitializer(() => {
      inject(ThemeService);
      return firstValueFrom(inject(CurrentUserService).loadCurrentUser());
    })
  ]
};


