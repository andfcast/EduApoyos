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
                throw new Exception("Credenciales inválidas."); // Evita dar pistas de si el correo existe o no
            }

            // 3. Generar el Token (JWT u otro mecanismo de sesión)
            string token = _tokenService.GenerateToken(usuario);

            return new LoginResponseDto
            {
                Token = token,
                Mensaje = "Autenticación exitosa.",
                UsuarioId = usuario.Id,
                Nombre = usuario.NombreCompleto
            };
        }

        public async Task<string> RegistrarEstudianteAsync(RegistroUsuarioDto dto)
        {
            var existeUsuario = await _unitOfWork.Usuarios.ObtenerPorCorreoAsync(dto.Email);
            if (existeUsuario != null)
            {
                throw new InvalidOperationException("El correo electrónico ya está en uso.");
            }

            // 2. Mapear y encriptar la entidad Base (User) de Domain
            var nuevoUsuario = new Usuario
            (Guid.NewGuid(), dto.NombreCompleto, dto.Email, _passwordHasher.Hash(dto.Password), 2, DateTime.UtcNow);

            // Agregamos al repositorio específico de usuarios
            await _unitOfWork.Usuarios.AgregarAsync(nuevoUsuario);

            // Guardamos aquí para que EF genere el nuevoUsuario.Id de forma segura
            await _unitOfWork.SaveChangesAsync();

            // 3. Mapear la entidad Extendida (Estudiante) usando el ID recién creado
            var nuevoEstudiante = new Estudiante(Guid.NewGuid(), nuevoUsuario.Id, dto.TipoDocumentoId, dto.NumeroDocumento, dto.ProgramaAcademico, dto.Semestre);

            // Agregamos al repositorio específico de estudiantes
            await _unitOfWork.Estudiantes.AgregarAsync(nuevoEstudiante);

            // Consolidamos la transacción atómica final en la base de datos
            await _unitOfWork.SaveChangesAsync();

            return "Estudiante registrado con éxito de manera segura.";
        }
    }
}
