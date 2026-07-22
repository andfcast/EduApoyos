import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Solicitud, RegistroSolicitud, CambioEstadoSolicitud } from '../models/solicitud.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SolicitudesService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}Solicitudes`;


  obtenerSolicitudes(): Observable<Solicitud[]> {
    return this.http.get<Solicitud[]>(this.apiUrl);
  }
  
  obtenerPorId(id: string): Observable<Solicitud> {
    return this.http.get<Solicitud>(`${this.apiUrl}/${id}`);
  }

  crearSolicitud(solicitud: RegistroSolicitud): Observable<RegistroSolicitud> {    
    return this.http.post<RegistroSolicitud>(this.apiUrl, solicitud);
  }

  cambiarEstadoSolicitud(id: string, nuevoEstadoId: number, usuarioId: string): Observable<void> {
    let cambioEstado: CambioEstadoSolicitud = { estadoId: nuevoEstadoId, usuarioId: usuarioId };
    return this.http.patch<void>(`${this.apiUrl}/${id}/estado`, cambioEstado);
  }
}