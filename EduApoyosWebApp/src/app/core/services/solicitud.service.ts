import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Solicitud, RegistroSolicitud, CambioEstadoSolicitud, FiltroSolicitud } from '../models/solicitud.models';
import { environment } from '../../../environments/environment';
import { RespuestaPaginada } from '../models/general.models';

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

  filtrarSolicitudes(filtro: FiltroSolicitud): Observable<RespuestaPaginada<Solicitud>> {
    let params = new HttpParams()
      .set('pagina', filtro.pagina.toString())
      .set('tamanoPagina', filtro.tamanoPagina.toString());

    if (filtro.tipoApoyoId) {
      params = params.set('tipoApoyoId', filtro.tipoApoyoId.toString());
    }
    if (filtro.fecha) {
      params = params.set('fecha', filtro.fecha);
    }
    if (filtro.estadoId) {
      params = params.set('estadoId', filtro.estadoId.toString());
    }

    // Realiza la petición GET /api/solicitudes?pagina=1&tamanoPagina=5&...
    return this.http.get<RespuestaPaginada<Solicitud>>(this.apiUrl, { params });
  }

  crearSolicitud(solicitud: RegistroSolicitud): Observable<RegistroSolicitud> {    
    return this.http.post<RegistroSolicitud>(this.apiUrl, solicitud);
  }

  cambiarEstadoSolicitud(id: string, nuevoEstadoId: number, usuarioId: string): Observable<void> {
    let cambioEstado: CambioEstadoSolicitud = { estadoId: nuevoEstadoId, usuarioId: usuarioId };
    return this.http.patch<void>(`${this.apiUrl}/${id}/estado`, cambioEstado);
  }
}