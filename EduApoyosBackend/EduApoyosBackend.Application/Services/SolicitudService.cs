using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Application.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SolicitudService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<SolicitudApoyoDto>> ObtenerSolicitudesAsync()
        {
            var solicitudes = await _unitOfWork.Solicitudes.ListarAsync();
            return solicitudes.Select(t => new SolicitudApoyoDto
            {
                Id = t.Id,
                Descripcion = t.Descripcion,
                Estado = t.EstadoSolicitud.Nombre,
                FechaSolicitud = t.FechaSolicitud,
                MontoSolicitado = t.MontoSolicitado,
                TipoApoyo = t.TipoApoyo.Nombre,
                NombreEstudiante = t.Estudiante.Usuario.NombreCompleto,
                NombreAsesor = t.Asesor!.NombreCompleto,
                FechaActualizacion = t.FechaActualizacion,
                HistorialEstados = t.HistorialEstados.Select(h => new HistorialEstadoDto
                {
                    Id = h.Id,
                    SolicitudId = h.SolicitudId,
                    EstadoAnterior = h.EstadoAnteriorId == null || h.EstadoAnteriorId == 0 ? string.Empty : (h.EstadoAnterior != null ? h.EstadoAnterior.Nombre : string.Empty),
                    EstadoSiguiente = h.EstadoNuevo != null ? h.EstadoNuevo.Nombre : string.Empty,
                    FechaCambio = h.FechaCambio,
                    Observacion = h.Observacion
                }).ToList()
            });

        }

        public async Task<SolicitudApoyoDto> ObtenerSolicitudAsync(Guid solicitudId)
        {
            var solicitud = await _unitOfWork.Solicitudes.ObtenerPorGuidAsync(solicitudId);
            if (solicitud == null)
            {
                throw new InvalidOperationException("La solicitud no existe.");
            }
            return new SolicitudApoyoDto
            {
                Id = solicitud.Id,
                Descripcion = solicitud.Descripcion,
                Estado = solicitud.EstadoSolicitud.Nombre,
                FechaSolicitud = solicitud.FechaSolicitud,
                MontoSolicitado = solicitud.MontoSolicitado,
                TipoApoyo = solicitud.TipoApoyo.Nombre,
                NombreEstudiante = solicitud.Estudiante.Usuario.NombreCompleto,
                NombreAsesor = solicitud.Asesor!.NombreCompleto,
                FechaActualizacion = solicitud.FechaActualizacion,
                HistorialEstados = solicitud.HistorialEstados.Select(h => new HistorialEstadoDto
                {
                    Id = h.Id,
                    SolicitudId = h.SolicitudId,
                    EstadoAnterior = h.EstadoAnteriorId == null || h.EstadoAnteriorId == 0 ? string.Empty : (h.EstadoAnterior != null ? h.EstadoAnterior.Nombre : string.Empty),
                    EstadoSiguiente = h.EstadoNuevo != null ? h.EstadoNuevo.Nombre : string.Empty,
                    FechaCambio = h.FechaCambio,
                    Observacion = h.Observacion
                }).ToList()
            };
        }

        public async Task<RespuestaPaginadaDto<SolicitudApoyoDto>> ObtenerSolicitudesFiltradasAsync(FiltroSolicitudDto filtro)
        {
           
            var query = _unitOfWork.Solicitudes.Query();

            if (filtro.TipoApoyoId.HasValue && filtro.TipoApoyoId.Value > 0)
            {
                query = query.Where(s => s.TipoApoyoId == filtro.TipoApoyoId.Value);
            }

            if (filtro.Fecha.HasValue)
            {
                var fechaFiltro = filtro.Fecha.Value.Date;
                query = query.Where(s => s.FechaSolicitud.Date == fechaFiltro);
            }

            if (filtro.EstadoId.HasValue && filtro.EstadoId.Value > 0)
            {
                query = query.Where(s => s.EstadoSolicitudId == filtro.EstadoId.Value);
            }

            var totalRegistros = await query.CountAsync();
            
            var elementos = await query
                .OrderByDescending(s => s.FechaSolicitud)
                .Skip((filtro.Pagina - 1) * filtro.TamanoPagina)
                .Take(filtro.TamanoPagina)
                .Select(solicitud => new SolicitudApoyoDto
                {
                    Id = solicitud.Id,
                    Descripcion = solicitud.Descripcion,
                    Estado = solicitud.EstadoSolicitud.Nombre,
                    FechaSolicitud = solicitud.FechaSolicitud,
                    MontoSolicitado = solicitud.MontoSolicitado,
                    TipoApoyo = solicitud.TipoApoyo.Nombre,
                    NombreEstudiante = solicitud.Estudiante.Usuario.NombreCompleto,
                    NombreAsesor = solicitud.Asesor != null ? solicitud.Asesor.NombreCompleto : string.Empty,
                    FechaActualizacion = solicitud.FechaActualizacion,
                    HistorialEstados = solicitud.HistorialEstados.Select(h => new HistorialEstadoDto
                    {
                        Id = h.Id,
                        SolicitudId = h.SolicitudId,
                        EstadoAnterior = h.EstadoAnteriorId == null || h.EstadoAnteriorId == 0 ? string.Empty : h.EstadoAnterior!.Nombre,
                        EstadoSiguiente = h.EstadoNuevo.Nombre,
                        FechaCambio = h.FechaCambio,
                        Observacion = h.Observacion
                    }).ToList()
                })
                .ToListAsync();
            
            return new RespuestaPaginadaDto<SolicitudApoyoDto>
            {
                Elementos = elementos,
                TotalRegistros = totalRegistros,
                PaginaActual = filtro.Pagina
            };
        }

        public async Task<string> RegistrarSolicitudAsync(RegistroSolicitudDto dto)
        {
            // Validar que el estudiante exista
            if (dto.EstudianteId == Guid.Empty)
                throw new InvalidOperationException("El Id del estudiante no es válido.");

            // Intentar buscar el estudiante por su Id (compatibilidad con Estudiante.Id)
            var estudianteExistente = await _unitOfWork.Estudiantes.ObtenerPorGuidAsync(dto.EstudianteId);
            // Si no se encuentra por Id, intentar por UsuarioId (compatibilidad hacia atrás)
            if (estudianteExistente == null)
            {
                var busquedaEstudiante = await _unitOfWork.Estudiantes.BuscarAsync(s => s.UsuarioId == dto.EstudianteId);
                estudianteExistente = busquedaEstudiante.FirstOrDefault();
            }

            if (estudianteExistente == null)
                throw new InvalidOperationException("El estudiante especificado no existe.");

            // Determinar asesor a asignar: si el DTO no trae AsesorId, intentar usar un asesor existente en BD
            Guid? asesorId = dto.AsesorId;
            if (asesorId == null || asesorId == Guid.Empty)
            {
                var posibles = await _unitOfWork.Usuarios.BuscarAsync(u => u.RolId == 1);
                var primerAsesor = posibles.FirstOrDefault();
                asesorId = primerAsesor != null ? primerAsesor.Id : (Guid?)null;
            }

            var solicitud = new Domain.Entities.SolicitudApoyo(
                Guid.NewGuid(),
                estudianteExistente.Id,
                dto.TipoApoyoId,
                dto.MontoSolicitado,
                dto.Descripcion,
                1, // Estado inicial: Pendiente
                DateTime.UtcNow,
                DateTime.UtcNow,
                asesorId
            );
            await _unitOfWork.Solicitudes.AgregarAsync(solicitud);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.HistorialEstados.AgregarAsync(new HistorialEstado(Guid.NewGuid(), solicitud.Id, 1, solicitud.EstadoSolicitudId, DateTime.UtcNow, asesorId, $"Solicitud registrada"));
            await _unitOfWork.SaveChangesAsync();
            return "Solicitud registrada con éxito de manera segura.";
        }
        public async Task<string> ActualizarEstadoSolicitudAsync(Guid solicitudId, ActualizarEstadoSolicitudDto dto)
        {
            var solicitud = await _unitOfWork.Solicitudes.ObtenerPorGuidAsync(solicitudId);
            if (solicitud == null)
            {
                throw new InvalidOperationException("La solicitud no existe.");
            }
            var estadoAnterior = solicitud.EstadoSolicitudId;
            int nuevoEstadoId = dto.EstadoId; // Usar el ID del estado proporcionado en el DTO
            solicitud.ActualizarEstado(nuevoEstadoId);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.HistorialEstados.AgregarAsync(new HistorialEstado(Guid.NewGuid(), solicitudId, estadoAnterior, nuevoEstadoId, DateTime.UtcNow, dto.UsuarioId, $"Cambio de estado de {estadoAnterior} a {nuevoEstadoId}"));
            await _unitOfWork.SaveChangesAsync();
            return "Estado de la solicitud actualizado con éxito.";
        }

        public async Task<IEnumerable<SolicitudApoyoDto>> ObtenerSolicitudesXEstudianteAsync(Guid id)
        {
            // Intentar obtener el estudiante por Id (el controlador pasa el Id del estudiante en la ruta).
            // Buscar por Id o por UsuarioId para compatibilidad hacia atrás y evitar problemas con FindAsync/capturas
            var estudiante = (await _unitOfWork.Estudiantes.BuscarAsync(x => x.Id == id || x.UsuarioId == id)).FirstOrDefault();
            if (estudiante == null)
                return Enumerable.Empty<SolicitudApoyoDto>();

            var estudianteId = estudiante.Id;
            // Construir la consulta y cargar explícitamente las relaciones necesarias con Include
            var query = _unitOfWork.Solicitudes
                .Buscar(x => x.EstudianteId == estudianteId)
                .AsQueryable();

            var solicitudes = await query
                .Include(s => s.EstadoSolicitud)
                .Include(s => s.TipoApoyo)
                .Include(s => s.Estudiante).ThenInclude(e => e.Usuario)
                .Include(s => s.Asesor)
                .Include(s => s.HistorialEstados).ThenInclude(h => h.EstadoAnterior)
                .Include(s => s.HistorialEstados).ThenInclude(h => h.EstadoNuevo)
                .ToListAsync();

            // Mapear con comprobaciones nulas para evitar NullReferenceException si faltan Includes o datos
            return solicitudes.Select(solicitud => new SolicitudApoyoDto
            {
                Id = solicitud.Id,
                Descripcion = solicitud.Descripcion,
                Estado = solicitud.EstadoSolicitud?.Nombre ?? "Pendiente",
                FechaSolicitud = solicitud.FechaSolicitud,
                MontoSolicitado = solicitud.MontoSolicitado,
                TipoApoyo = solicitud.TipoApoyo?.Nombre ?? string.Empty,
                NombreEstudiante = solicitud.Estudiante?.Usuario?.NombreCompleto ?? string.Empty,
                NombreAsesor = solicitud.Asesor?.NombreCompleto ?? string.Empty,
                FechaActualizacion = solicitud.FechaActualizacion,
                HistorialEstados = solicitud.HistorialEstados?.Select(h => new HistorialEstadoDto
                {
                    Id = h.Id,
                    SolicitudId = h.SolicitudId,
                    EstadoAnterior = h.EstadoAnteriorId == 0 ? string.Empty : h.EstadoAnterior?.Nombre ?? string.Empty,
                    EstadoSiguiente = h.EstadoNuevo?.Nombre ?? string.Empty,
                    FechaCambio = h.FechaCambio,
                    Observacion = h.Observacion
                }).ToList() ?? new List<HistorialEstadoDto>()
            }).ToList();
        }
    }
}
