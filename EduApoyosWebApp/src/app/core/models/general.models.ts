export interface General{
   id: number;  
  nombre: string;
}
export interface TipoDocumento extends General {  
  codigo: string;  
}

export interface ProgramaAcademico extends General {}

export interface TipoApoyo extends General {}

export interface EstadoSolicitud extends General {}

export interface RespuestaPaginada<T> {
  elementos: T[];
  totalRegistros: number;
  paginaActual: number;  
}

