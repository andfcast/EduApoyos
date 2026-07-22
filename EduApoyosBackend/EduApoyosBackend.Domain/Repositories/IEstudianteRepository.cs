using EduApoyosBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Domain.Repositories
{
    public interface IEstudianteRepository : IGenericRepository<Estudiante>
    {
        Task<bool> ExisteUsuarioPorNumDocumentoAsync(int tipoDocumento, string numDocumento);
        Task<IEnumerable<Estudiante>> ListarConRelacionesAsync();
    }
}
