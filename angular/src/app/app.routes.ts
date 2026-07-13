import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
  },
  {
    path: 'favoritos',
    loadComponent: () => import('./favoritos/favoritos.component').then(c => c.FavoritosComponent),
    canActivate: [authGuard],
  },
  {
    path: 'auth/login',
    loadComponent: () => import('./account/login/login').then(c => c.Login),
  },
  {
    path: 'auth/register',
    loadComponent: () => import('./account/register/register').then(c => c.Register),
  },
  {
    path: 'profile/:id',
    loadComponent: () => import('./profile/public-profile/public-profile').then(c => c.PublicProfile),
  },
  {
    path: 'viajeros',
    loadComponent: () => import('./profile/user-search/user-search').then(c => c.UserSearchComponent),
  },
  {
    path: 'destinos',
    loadChildren: () => import('./destinos/destinos.routes').then(m => m.DESTINATIONS_ROUTES),
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
  {
    path: 'identity',
    loadChildren: () => import('@abp/ng.identity').then(c => c.createRoutes()),
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
  },
];
