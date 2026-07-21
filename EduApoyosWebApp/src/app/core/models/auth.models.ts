export interface LoginDto {
  email: string;
  password: string;
}

export interface AuthResponseDto {
  token: string;
  email: string;
  nombre: string;
}

export interface SesionUsuario {
  nombre: string;
  email: string;
  rol: 'Asesor' | 'Estudiante';
  token: string;
}