export interface RegistroUsuario {  
  nombreCompleto: string;  
  email: string;
  password: string;
  rolId: 1 | 2; // 1 para Asesor, 2 para Estudiante
}

export interface Rol {
  id: number;
  nombre: string;  
}