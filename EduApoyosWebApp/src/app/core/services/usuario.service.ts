import { Service } from '@angular/core';
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegistroEstudiante,Estudiante } from '../models/estudiante.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UsuarioService {
    private http = inject(HttpClient);
  // Toma la URL base definida en environment.ts/development.ts y le concatena la ruta
  private apiAuthUrl = `${environment.apiUrl}/auth`;
  private apiEstudianteUrl = `${environment.apiUrl}/estudiantes`;

  /**
   * Registra un nuevo estudiante en el sistema.
   * @param estudiante Objeto con la información requerida (documento, nombres, apellidos, email)
   */
  registrar(estudiante: RegistroEstudiante): Observable<RegistroEstudiante> {
    return this.http.post<RegistroEstudiante>(this.apiAuthUrl + '/Register', estudiante);
  }

  /**
   * Obtiene la información de un estudiante dado su ID.
   * (Requiere token de autenticación que adjuntará automáticamente el jwtInterceptor)
   * @param id Identificador único del estudiante
   */
  obtenerPorId(id: number): Observable<Estudiante> {
    return this.http.get<Estudiante>(`${this.apiEstudianteUrl}/${id}`);
  }
}
