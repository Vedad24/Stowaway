import { NgModule, Optional, SkipSelf } from '@angular/core';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { withCredentialsInterceptor } from './services/withCredentialsInterceptor';
import { xsrfInterceptor } from './services/xsrfInterceptor';
import { refreshInterceptor } from './services/refreshInterceptor';

@NgModule({
  providers: [
    provideHttpClient(
      withFetch(),
      withInterceptors([withCredentialsInterceptor, xsrfInterceptor, refreshInterceptor]),
    ),
  ],
})
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    if (parentModule) {
      throw new Error('CoreModule is already loaded. Import it only once, in the root providers.');
    }
  }
}
