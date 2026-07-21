export interface LoginDto {
  email: string;
  password: string;
}

export interface AuthResponseDto {
  token: string;
  email: string;
  nombre: string;
}