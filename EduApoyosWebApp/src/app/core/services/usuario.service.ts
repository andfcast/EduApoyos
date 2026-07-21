import { Service } from '@angular/core';
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { RegistroUsuario } from '../models/usuario.models';

@Injectable({
  providedIn: 'root'
})
export class UsuarioService {
    private http = inject(HttpClient);
  // Toma la URL base definida en environment.ts/development.ts y le concatena la ruta
  private apiUrl = `${environment.apiUrl}auth`;
  
  /**
   * Registra un nuevo usuario en el sistema.
   * @param usuario Objeto con la información requerida (nombreCompleto, email, password, rolId)
   */
  registrar(usuario: RegistroUsuario): Observable<RegistroUsuario> {
    return this.http.post<RegistroUsuario>(this.apiUrl + '/Register', usuario);
  }

}
