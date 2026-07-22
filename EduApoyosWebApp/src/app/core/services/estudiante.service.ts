import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { RegistroEstudiante, Estudiante, EstudianteCombo } from '../models/estudiante.models';
import { environment } from '../../../environments/environment';
import { Solicitud } from '../models/solicitud.models';

@Injectable({
  providedIn: 'root'
})
export class EstudianteService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}estudiantes`;

  obtenerPorId(id: number): Observable<Estudiante> {
    return this.http.get<Estudiante>(`${this.apiUrl}/${id}`);
  }

  obtenerTodos(): Observable<Estudiante[]> {
    return this.http.get<Estudiante[]>(this.apiUrl);
  }

  obtenerTodosCombo(): Observable<EstudianteCombo[]> {
    return this.http.get<Estudiante[]>(this.apiUrl).pipe(
      map((estudiantes: Estudiante[]): EstudianteCombo[] =>
        estudiantes.map((estudiante): EstudianteCombo => ({
          id: estudiante.id,
          nombreCompleto: estudiante.nombreCompleto
        }))
      )
    );
  }

  obtenerSolicitudesXEstudiante(estudianteId: string): Observable<Solicitud[]> {    
    return this.http.get<Solicitud[]>(this.apiUrl + '/' + estudianteId + '/solicitudes');
  }

  registrar(estudiante: RegistroEstudiante): Observable<RegistroEstudiante> {    
    return this.http.post<RegistroEstudiante>(this.apiUrl, estudiante);
  }

  actualizar(estudiante: RegistroEstudiante): Observable<RegistroEstudiante> {    
    return this.http.put<RegistroEstudiante>(this.apiUrl + '/' + estudiante.id, estudiante);
  }
}
