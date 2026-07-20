using EduApoyosBackend.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        public IUsuarioRepository Usuarios => throw new NotImplementedException();

        public IRolRepository Roles => throw new NotImplementedException();

        public ITipoDocumentoRepository TiposDocumento => throw new NotImplementedException();

        public ITipoApoyoRepository TiposApoyo => throw new NotImplementedException();

        public IEstadoSolicitudRepository EstadosSolicitud => throw new NotImplementedException();

        public IEstudianteRepository Estudiantes => throw new NotImplementedException();

        public ISolicitudApoyoRepository Solicitudes => throw new NotImplementedException();

        public IHistorialEstadoRepository HistorialEstados => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
