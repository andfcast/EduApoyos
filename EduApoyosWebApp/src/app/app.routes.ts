import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { publicOnlyGuard } from './core/guards/public-only.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { 
    path: 'login', 
    canActivate: [publicOnlyGuard],
    loadComponent: () => import('./features/auth/login-component/login-component').then(m => m.LoginComponent) 
  },
  { 
    path: 'registro', // 👈 Ruta que se enlaza desde el Login
    loadComponent: () => import('./features/auth/registrar-usuario-component/registrar-usuario-component').then(m => m.RegistrarUsuarioComponent) 
  },
  // Contenedor principal protegido por autenticación
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./layouts/main-layout-component/main-layout-component').then(m => m.MainLayoutComponent),
    children: [
      // Rutas para Asesor
      {
        path: 'asesor',
        canActivate: [roleGuard('Asesor')],
        loadComponent: () => import('./features/dashboard/asesor-dashboard-component/asesor-dashboard-component').then(m => m.AsesorDashboardComponent)
      }    
    ]
  },
  { path: '**', redirectTo: 'login' }
];
