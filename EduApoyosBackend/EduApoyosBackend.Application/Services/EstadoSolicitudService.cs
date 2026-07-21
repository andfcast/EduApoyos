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
    public class EstadoSolicitudService : IEstadoSolicitudService
    {
        private readonly IUnitOfWork _unitOfWork;
        public EstadoSolicitudService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<EstadoSolicitudDto>> ObtenerEstadosSolicitudAsync()
        {
            var estados = await _unitOfWork.EstadosSolicitud.ListarAsync();
            return estados.Select(e => new EstadoSolicitudDto
            {
                Id = e.Id,
                Nombre = e.Nombre
            });
        }
    }
}
