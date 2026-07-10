import { RoutesService, eLayoutType } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

function configureRoutes() {
  const routes = inject(RoutesService);
  routes.add([
      {
        path: '/',
        name: '::Menu:Home',
        iconClass: 'fas fa-home',
        order: 1,
        layout: eLayoutType.application,
      },
      {
        path: '/favoritos',
        name: '::Menu:Favoritos',
        iconClass: 'fas fa-heart',
        order: 2,
        layout: eLayoutType.application,
        requiredPolicy: 'ExploraYa1',
      },
      {
        path: '/destinos',
        name: '::Menu:Destinos',
        iconClass: 'fas fa-map-marked-alt',
        order: 3,
        layout: eLayoutType.application,
      },
      {
        path: '/experiencias',
        name: '::Menu:Experiencias',
        iconClass: 'fas fa-camera-retro',
        order: 4,
        layout: eLayoutType.application,
      },
  ]);
}
