using EduApoyosBackend.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Domain.Entities
{
    public class SolicitudApoyo : BaseEntity<Guid>
    {
        public Guid EstudianteId { get; private set; }
        public Estudiante Estudiante { get; private set; } = null!;
        public int TipoApoyoId { get; private set; }
        public TipoApoyo TipoApoyo { get; private set; } = null!;
        public string Descripcion { get; private set; }
        public int EstadoSolicitudId { get; private set; }
        public EstadoSolicitud EstadoSolicitud { get; private set; } = null!;
        public DateTime FechaSolicitud { get; private set; }
        public DateTime FechaActualizacion { get; private set; }
        public ICollection<HistorialEstado> HistorialEstados { get; private set; } = new List<HistorialEstado>();

        public SolicitudApoyo(Guid id, Guid estudianteId, int tipoApoyoId, string descripcion, int estadoSolicitudId, DateTime fechaSolicitud, DateTime fechaActualizacion)
        {
            Id = id; EstudianteId = estudianteId; TipoApoyoId = tipoApoyoId; Descripcion = descripcion; EstadoSolicitudId = estadoSolicitudId; FechaSolicitud = fechaSolicitud; FechaActualizacion = fechaActualizacion;
        }

        public void ActualizarEstado(int nuevoEstadoId)
        {
            EstadoSolicitudId = nuevoEstadoId;
            FechaActualizacion = DateTime.UtcNow;
        }
    }
}
