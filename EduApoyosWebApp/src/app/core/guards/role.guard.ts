import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard = (rolRequerido: 'Asesor' | 'Estudiante'): CanActivateFn => {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    const user = authService.currentUser();

    if (user && user.rol === rolRequerido) {
      return true;
    }

    // Si está autenticado pero no tiene el rol correcto, redirige a su área correspondiente
    if (user) {
      authService.redirigirSegunRol(user.rol);
      return false;
    }

    return router.createUrlTree(['/login']);
  };
};