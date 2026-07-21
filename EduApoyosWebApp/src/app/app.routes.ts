import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { 
    path: 'login', 
    loadComponent: () => import('./features/auth/login-component/login-component').then(m => m.LoginComponent) 
  },
  { 
    path: 'nuevo-usuario', 
    loadComponent: () => import('./features/auth/nuevo-usuario-component/nuevo-usuario-component').then(m => m.NuevoUsuarioComponent) 
  },
  { path: '**', redirectTo: 'login' }
];
