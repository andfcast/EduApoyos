import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TipoDocumento } from '../models/general.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TipoDocumentoService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}tiposdocumento`;

  obtenerTodos(): Observable<TipoDocumento[]> {
    return this.http.get<TipoDocumento[]>(this.apiUrl);
  }
}