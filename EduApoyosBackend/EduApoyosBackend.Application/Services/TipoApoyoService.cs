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
    public class TipoApoyoService : ITipoApoyoService
    {
        private readonly IUnitOfWork _unitOfWork;
        public TipoApoyoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<TipoApoyoDto>> ObtenerTiposApoyoAsync()
        {
            var tipos = await _unitOfWork.TiposApoyo.ListarAsync();
            return tipos.Select(t => new TipoApoyoDto
            {
                Id = t.Id,
                Nombre = t.Nombre
            });
        }
    }
}
