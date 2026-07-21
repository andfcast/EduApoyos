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
    path: 'registro',
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
      },
      {
        path: 'estudiantes',
        canActivate: [roleGuard('Asesor')],
        loadComponent: () => import('./features/estudiantes/lista-estudiantes-component/lista-estudiantes-component').then(m => m.ListaEstudiantesComponent)
      }, 

      // Rutas para Estudiante
      {
        path: 'estudiante',
        canActivate: [roleGuard('Estudiante')],
        loadComponent: () => import('./features/dashboard/estudiante-dashboard-component/estudiante-dashboard-component').then(m => m.EstudianteDashboardComponent)
      },    
    ]
  },
  { path: '**', redirectTo: 'login' }
];
