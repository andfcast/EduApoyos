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
    public class EstudianteService: IEstudianteService
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
    }
}
