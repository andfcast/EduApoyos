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
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public UsuarioService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
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

            //Si es estudiante, se crea preliminarmente, pero se puede editar posteriormente para agregar los datos faltantes, como el tipo de documento,
            //número de documento, programa académico y semestre.
            if (dto.RolId == 2) { 
                var nuevoEstudiante = new Estudiante(Guid.NewGuid(), nuevoUsuario.Id,1,"",1,1);
                await _unitOfWork.Estudiantes.AgregarAsync(nuevoEstudiante);
                await _unitOfWork.SaveChangesAsync();
            }

            return "Usuario registrado con éxito de manera segura.";
        }
    }
}
