export interface Solicitud {
    id:string;
    nombreEstudiante: string;
    tipoApoyo: string;
    montoSolicitado: number;
    descripcion: string;
    estado: string;
    fechaSolicitud: Date;
    nombreAsesor: string;
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