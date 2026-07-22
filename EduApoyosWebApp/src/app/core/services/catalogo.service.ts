import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EstadoSolicitud, ProgramaAcademico, TipoDocumento } from '../models/general.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CatalogoService {
  private http = inject(HttpClient);
  private apiTipoDocumentoUrl = `${environment.apiUrl}tiposdocumento`;
  private apiProgramaAcademicoUrl = `${environment.apiUrl}programaAcademico`;
  private apiEstadosSolicitudUrl = `${environment.apiUrl}estadosSolicitud`;

  obtenerTiposDocumento(): Observable<TipoDocumento[]> {
    return this.http.get<TipoDocumento[]>(this.apiTipoDocumentoUrl);
  }

  obtenerProgramasAcademicos(): Observable<ProgramaAcademico[]> {
    return this.http.get<ProgramaAcademico[]>(this.apiProgramaAcademicoUrl);
  }

  obtenerEstadosSolicitud(): Observable<EstadoSolicitud[]> {
    return this.http.get<EstadoSolicitud[]>(this.apiEstadosSolicitudUrl);
  }
}