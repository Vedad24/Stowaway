import { ApplicationConfig, importProvidersFrom, inject, LOCALE_ID, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { registerLocaleData } from '@angular/common';
import localeBs from '@angular/common/locales/bs';
import { firstValueFrom } from 'rxjs';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { ThemeService } from './services/theme-service';
import { CurrentUserService } from './services/identity/auth/current-user-service';
import { CoreModule } from './core.module';

registerLocaleData(localeBs);

export const appConfig: ApplicationConfig = {
  providers: [
    { provide: LOCALE_ID, useValue: 'bs' },
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideClientHydration(withEventReplay()),
    importProvidersFrom(CoreModule),
    provideAppInitializer(() => {
      inject(ThemeService);
      return firstValueFrom(inject(CurrentUserService).loadCurrentUser());
    })
  ]
};


