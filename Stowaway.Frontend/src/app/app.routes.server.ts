import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  {
    path: '',
    renderMode: RenderMode.Server
  },
  {
    path: 'product-page',
    renderMode: RenderMode.Server
  },
  //Everything else needs auth/browser context -> Client
  {
    path: '**',
    renderMode: RenderMode.Client
  }
];
