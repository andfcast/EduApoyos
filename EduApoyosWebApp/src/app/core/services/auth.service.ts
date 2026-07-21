import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthResponseDto, LoginDto, SesionUsuario } from '../models/auth.models';
import { environment } from '../../../environments/environment';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = `${environment.apiUrl}/auth`; // Reemplaza por tu puerto local de la API
  private tokenKey = 'eduapoyos_jwt_token';
  private sessionKey = 'eduapoyos_session';
  private readonly hasLocalStorage = typeof localStorage !== 'undefined';

  currentUser = signal<SesionUsuario | null>(this.obtenerSesionGuardada());
  isLoggedIn = computed(() => !!this.currentUser());
  userRole = computed(() => this.currentUser()?.rol ?? null);

  login(credentials: LoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        if (response?.token) {
          this.setToken(response.token);
        }
      })
    );
  }

  setToken(token: string): void {
    if (!this.hasLocalStorage) {
      return;
    }
    localStorage.setItem(this.tokenKey, token);
  }

  getToken(): string | null {
    return this.currentUser()?.token ?? null;
  }
  
  logout(): void {
    if (this.hasLocalStorage) {
      localStorage.removeItem(this.sessionKey);
    }
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  public redirigirSegunRol(rol: string): void {
    if (rol === 'Asesor') {
      this.router.navigate(['/dashboard/asesor']);
    } else if (rol === 'Estudiante') {
      this.router.navigate(['/dashboard/estudiante']);
    } else {
      this.router.navigate(['/login']);
    }
  }

  private guardarSesion(session: SesionUsuario): void {
    if (!this.hasLocalStorage) {
      return;
    }
    localStorage.setItem(this.sessionKey, JSON.stringify(session));
  }

  private obtenerSesionGuardada(): SesionUsuario | null {
    if (!this.hasLocalStorage) {
      return null;
    }
    const data = localStorage.getItem(this.sessionKey);
    return data ? JSON.parse(data) : null;
  }

  
}