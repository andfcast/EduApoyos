using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Domain.Repositories;
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
                NombreAsesor = t.Asesor.NombreCompleto,
                FechaActualizacion = t.FechaActualizacion,
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
                NombreAsesor = solicitud.Asesor.NombreCompleto,
                FechaActualizacion = solicitud.FechaActualizacion,
                HistorialEstados = solicitud.HistorialEstados.Select(h => new HistorialEstadoDto
                {
                    Id = h.Id,
                    SolicitudId = h.SolicitudId,
                    EstadoAnterior = h.EstadoAnteriorId == 0 ? string.Empty : h.EstadoAnterior.Nombre,
                    EstadoSiguiente = h.EstadoNuevo.Nombre,
                    FechaCambio = h.FechaCambio,                    
                    Observacion = h.Observacion
                }).ToList()
            };
        }

        public async Task<string> RegistrarSolicitudAsync(RegistroSolicitudDto dto)
        {
            var solicitud = new Domain.Entities.SolicitudApoyo(
                Guid.NewGuid(),
                dto.EstudianteId,
                dto.TipoApoyoId,
                dto.Descripcion,
                1, // Estado inicial: Pendiente
                DateTime.UtcNow,
                DateTime.UtcNow,
                dto.AsesorId
            );
            await _unitOfWork.Solicitudes.AgregarAsync(solicitud);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.HistorialEstados.AgregarAsync(new HistorialEstado(Guid.NewGuid(), solicitud.Id, 1, solicitud.EstadoSolicitudId, DateTime.UtcNow, dto.AsesorId, $"Solicitud registrada"));
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
    }
}
