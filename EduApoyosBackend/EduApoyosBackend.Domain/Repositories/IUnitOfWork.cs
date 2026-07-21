using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Domain.Repositories
{
    public interface IUnitOfWork
    {
        IUsuarioRepository Usuarios { get; }
        IRolRepository Roles { get; }
        ITipoDocumentoRepository TiposDocumento { get; }
        IProgramaAcademicoRepository ProgramasAcademicos { get; }
        ITipoApoyoRepository TiposApoyo { get; }
        IEstadoSolicitudRepository EstadosSolicitud { get; }
        IEstudianteRepository Estudiantes { get; }
        ISolicitudApoyoRepository Solicitudes { get; }
        IHistorialEstadoRepository HistorialEstados { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
