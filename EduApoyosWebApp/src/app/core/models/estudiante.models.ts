export interface Estudiante {
  id?: number;
  documento: string;
  nombres: string;
  apellidos: string;
  email: string;
  usuarioId?: number;
}

export interface RegistroEstudiante {  
  nombreCompleto: string;
  email: string;
  password:string;
  tipoDocumentoId:number;
  numeroDocumento: string;    
  programaAcademico:string;
  semestre:number; 
  activo: boolean;   
}