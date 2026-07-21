export interface LoginDto {
  email: string;
  password: string;
}

export interface SesionUsuario {
  token: string;
  Mensaje: string;
  nombre: string;
  usuarioId: string;
  email: string;
  rol: 'Asesor' | 'Estudiante';  
}