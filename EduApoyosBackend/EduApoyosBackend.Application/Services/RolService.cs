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
    public class RolService : IRolService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RolService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<RolDto>> ObtenerRolesActivosAsync()
        {
            var roles = await _unitOfWork.Roles.ListarAsync();

            return roles.Select(r => new RolDto
            {
                Id = r.Id,
                Nombre = r.Nombre
            });
        }
    }
}

                
