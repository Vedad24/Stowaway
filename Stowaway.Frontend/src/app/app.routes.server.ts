import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  {
    path: 'warehouse/edit/:abc',
    renderMode: RenderMode.Server
  },
  {
    path: 'item/edit/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'supplier/edit/:id',
    renderMode: RenderMode.Server
  },
  {
    path: '**',
    renderMode: RenderMode.Prerender
  }
];
