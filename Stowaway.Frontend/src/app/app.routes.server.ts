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
  //Any paths that require something from backend with auth should use RenderMode.Client
  {
    path: 'employee-management',
    renderMode: RenderMode.Client
  },
  {
    path: '**',
    renderMode: RenderMode.Prerender
  }
];
