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
    public class TipoDocumentoService: ITipoDocumentoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TipoDocumentoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TipoDocumentoDto>> ObtenerTiposDocumentoActivosAsync()
        {
            var tipos = await _unitOfWork.TiposDocumento.ListarAsync();

            return tipos.Select(t => new TipoDocumentoDto
            {
                Id = t.Id,
                Codigo = t.Codigo,
                Nombre = t.Descripcion
            });
        }
    }
}
