using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Domain.Repositories;
using EduApoyosBackend.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Infrastructure.Persistence.Repositories
{
    public class EstudianteRepository : GenericRepository<Estudiante>, IEstudianteRepository
    {
        public EstudianteRepository(AppDbContext context) : base(context) { }

        public async Task<bool> ExisteUsuarioPorNumDocumentoAsync(int tipoDocumento, string numDocumento)
        {
            return await _context.Estudiantes.AnyAsync(e => e.TipoDocumentoId == tipoDocumento && e.NumeroDocumento == numDocumento);
        }
    }
}
