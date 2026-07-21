using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }
        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var usuario = await _unitOfWork.Usuarios.ObtenerPorCorreoAsync(dto.Email);
            if (usuario == null || !_passwordHasher.Verify(dto.Password, usuario.PasswordHash))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas."); // Evita dar pistas de si el correo existe o no
            }

            // 3. Generar el Token (JWT u otro mecanismo de sesión)
            string token = _tokenService.GenerateToken(usuario);

            return new LoginResponseDto
            {
                Token = token,
                Mensaje = "Autenticación exitosa.",
                UsuarioId = usuario.Id,
                Nombre = usuario.NombreCompleto,
                Email = usuario.Email,
                Rol = usuario.Rol?.Nombre ?? (usuario.RolId == 1 ? "Asesor" : "Estudiante")
            };
        }

        public async Task<string> RegistrarUsuarioAsync(RegistroUsuarioDto dto)
        {
            var existeUsuario = await _unitOfWork.Usuarios.ObtenerPorCorreoAsync(dto.Email);
            if (existeUsuario != null)
            {
                throw new InvalidOperationException("El correo electrónico ya está en uso.");
            }            
            var nuevoUsuario = new Usuario(Guid.NewGuid(), dto.NombreCompleto, dto.Email, _passwordHasher.Hash(dto.Password), dto.RolId, DateTime.UtcNow);

            await _unitOfWork.Usuarios.AgregarAsync(nuevoUsuario);

            await _unitOfWork.SaveChangesAsync();            

            return "Usuario registrado con éxito de manera segura.";
        }
    }
}
