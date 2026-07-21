using EduApoyosBackend.Domain.Repositories;
using EduApoyosBackend.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
    {
        private readonly AppDbContext _context;
        private bool _disposed = false;

        private IUsuarioRepository? _usuarios;

        public IUsuarioRepository Usuarios => _usuarios ??= new Repositories.UsuarioRepository(_context);

        private IRolRepository? _roles;
        public IRolRepository Roles => _roles ??= new Repositories.RolRepository(_context);

        private ITipoDocumentoRepository? _tiposDocumento;
        public ITipoDocumentoRepository TiposDocumento => _tiposDocumento ??= new Repositories.TipoDocumentoRepository(_context);

        private IProgramaAcademicoRepository? _programasAcademicos;
        public IProgramaAcademicoRepository ProgramasAcademicos => _programasAcademicos ??= new Repositories.ProgramaAcademicoRepository(_context);

        private ITipoApoyoRepository? _tiposApoyo;
        public ITipoApoyoRepository TiposApoyo => _tiposApoyo ??= new Repositories.TipoApoyoRepository(_context);

        private IEstadoSolicitudRepository? _estadosSolicitud;
        public IEstadoSolicitudRepository EstadosSolicitud => _estadosSolicitud ??= new Repositories.EstadoSolicitudRepository(_context);

        private IEstudianteRepository? _estudiantes;
        public IEstudianteRepository Estudiantes => _estudiantes ??= new Repositories.EstudianteRepository(_context);

        private ISolicitudApoyoRepository? _solicitudes;
        public ISolicitudApoyoRepository Solicitudes => _solicitudes ??= new Repositories.SolicitudApoyoRepository(_context);

        private IHistorialEstadoRepository? _historialEstados;
        public IHistorialEstadoRepository HistorialEstados => _historialEstados ??= new Repositories.HistorialEstadoRepository(_context);

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        // Liberación sincrónica de recursos
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Liberación asincrónica requerida por el contenedor de dependencias de ASP.NET Core
        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                if (_context is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else
                {
                    _context.Dispose();
                }

                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
