using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using EduApoyosBackend.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Application.Services
{
    public class ProgramaAcademicoService : IProgramaAcademicoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProgramaAcademicoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProgramaAcademicoDto>> ObtenerProgramasAcademicosActivosAsync()
        {
            var programas = await _unitOfWork.ProgramasAcademicos.ListarAsync();

            return programas.Select(p => new ProgramaAcademicoDto
            {
                Id = p.Id,                
                Nombre = p.Descripcion
            });
        }
    }
}
