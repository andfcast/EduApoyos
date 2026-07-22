export interface Solicitud {
    id:string;
    nombreEstudiante: string;
    tipoApoyo: string;
    montoSolicitado: number;
    descripcion: string;
    estado: string;    
    fechaSolicitud: Date;
    fechaActualizacion: Date;
    nombreAsesor: string;
    historialEstados: HistorialEstadoSolicitud[];
}
 
export interface RegistroSolicitud {
    id:string;
    estudianteId: string;
    tipoApoyoId: number;
    montoSolicitado: number;
    descripcion: string;
    estadoId: string;
    fechaSolicitud: Date;
    asesorId: string;
}

export interface CambioEstadoSolicitud {
    estadoId: number;
    usuarioId: string;
}

export interface HistorialEstadoSolicitud {
    id: string;
    solicitudId: string;
    estadoAnterior: string;
    estadoNuevo: string;
    fechaCambio: Date;
    observacion: string;
}

export interface FiltroSolicitud {
  estadoId?: number | null;
  tipoApoyoId?: number | null;
  fecha?: string | null;  
  pagina: number;        
  tamanoPagina: number;
}