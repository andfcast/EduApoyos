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
    public class EstudianteService : IEstudianteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public EstudianteService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<IEnumerable<EstudianteDto>> ObtenerEstudiantesAsync()
        {
            var estudiantes = await _unitOfWork.Estudiantes.ListarConRelacionesAsync();
            return estudiantes.Select(e => new EstudianteDto            
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                TipoDocumento = e.TipoDocumento.Descripcion,
                TipoDocumentoId = e.TipoDocumentoId,
                NumeroDocumento = e.NumeroDocumento,
                NombreCompleto = e.Usuario.NombreCompleto,
                Email = e.Usuario.Email,
                ProgramaAcademicoId = e.ProgramaAcademicoId,
                ProgramaAcademico = e.ProgramaAcademico.Descripcion,
                Semestre = e.Semestre,
                Activo = e.Activo
            });
        }

        public async Task<string> RegistrarEstudianteAsync(RegistroEstudianteDto dto)
        {
            var existeUsuario = await _unitOfWork.Usuarios.ObtenerPorCorreoAsync(dto.Email);
            if (existeUsuario != null)
            {
                throw new InvalidOperationException("El correo electrónico ya está en uso.");
            }
            var existeEstudiante = await _unitOfWork.Estudiantes.ExisteUsuarioPorNumDocumentoAsync(dto.TipoDocumentoId, dto.NumeroDocumento);
            if (existeEstudiante)
            {
                throw new InvalidOperationException("El número de documento ya está en uso.");
            }

            var nuevoUsuario = new Usuario(Guid.NewGuid(), dto.NombreCompleto, dto.Email, _passwordHasher.Hash(dto.Password), 2, DateTime.UtcNow);

            await _unitOfWork.Usuarios.AgregarAsync(nuevoUsuario);

            await _unitOfWork.SaveChangesAsync();

            var nuevoEstudiante = new Estudiante(Guid.NewGuid(), nuevoUsuario.Id, dto.TipoDocumentoId, dto.NumeroDocumento, dto.ProgramaAcademicoId, dto.Semestre);

            await _unitOfWork.Estudiantes.AgregarAsync(nuevoEstudiante);

            await _unitOfWork.SaveChangesAsync();

            return "Estudiante registrado con éxito de manera segura.";
        }

        public async Task<string> ActualizarEstudianteAsync(EdicionEstudianteDto dto)
        {
            var estudiante = await _unitOfWork.Estudiantes.ObtenerPorGuidAsync(dto.Id);
            if (estudiante == null)
            {
                throw new InvalidOperationException("El estudiante no fue encontrado.");
            }

            // Actualizar los datos del estudiante
            estudiante.TipoDocumentoId = dto.TipoDocumentoId;
            estudiante.NumeroDocumento = dto.NumeroDocumento;
            estudiante.ProgramaAcademicoId = dto.ProgramaAcademicoId;
            estudiante.Semestre = dto.Semestre;
            estudiante.Activo = dto.Activo;

            var usuarioExistente = await _unitOfWork.Usuarios.ObtenerPorGuidAsync(estudiante.UsuarioId);
            if (usuarioExistente != null)
            {
                usuarioExistente.NombreCompleto = dto.NombreCompleto;
                usuarioExistente.Email = dto.Email;
                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    usuarioExistente.PasswordHash = _passwordHasher.Hash(dto.Password);
                }
                _unitOfWork.Usuarios.Actualizar(usuarioExistente);
                await _unitOfWork.SaveChangesAsync();
            }
            
            _unitOfWork.Estudiantes.Actualizar(estudiante);
            await _unitOfWork.SaveChangesAsync();

            return "Estudiante actualizado con éxito.";
        }
    }
}
