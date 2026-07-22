export interface Estudiante {
  id: string;
  tipoDocumentoId: number;
  tipoDocumento: string;
  numeroDocumento: string;
  nombreCompleto: string;  
  email: string;
  programaAcademicoId: number;
  programaAcademico: string;
  semestre: number;
  activo: boolean;  
  usuarioId: string;
}

export interface RegistroEstudiante {  
  id: string;
  nombreCompleto: string;
  email: string;
  password:string;
  tipoDocumentoId:number;
  numeroDocumento: string;    
  programaAcademico:string;
  programaAcademicoId:number;
  semestre:number; 
  activo: boolean;   
}

export interface EstudianteCombo{
  id:string;
  nombreCompleto:string;
}