import { ApplicationConfig, importProvidersFrom, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { firstValueFrom } from 'rxjs';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { ThemeService } from './services/theme-service';
import { CurrentUserService } from './services/identity/auth/current-user-service';
import { CoreModule } from './core.module';

export const appConfig: ApplicationConfig = {
  providers: [
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


