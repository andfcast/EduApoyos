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
        public Guid EstudianteId { get; set; }
        public Estudiante Estudiante { get; set; } = null!;
        public int TipoApoyoId { get; set; }
        public TipoApoyo TipoApoyo { get; set; } = null!;
        public double MontoSolicitado { get; set; }
        public string Descripcion { get; set; }
        public int EstadoSolicitudId { get; set; }
        public Guid? AsesorId { get; set; }
        public Usuario? Asesor { get; set; }
        public EstadoSolicitud EstadoSolicitud { get; set; } = null!;
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public ICollection<HistorialEstado> HistorialEstados { get; set; } = new List<HistorialEstado>();

        private SolicitudApoyo() { }
        public SolicitudApoyo(Guid id, Guid estudianteId, int tipoApoyoId, double monto, string descripcion, int estadoSolicitudId, DateTime fechaSolicitud, DateTime fechaActualizacion, 
                                Guid? asesorId)
        {
            Id = id; EstudianteId = estudianteId; 
            TipoApoyoId = tipoApoyoId; 
            MontoSolicitado = monto;
            Descripcion = descripcion; 
            EstadoSolicitudId = estadoSolicitudId; 
            FechaSolicitud = fechaSolicitud; 
            FechaActualizacion = fechaActualizacion;
            AsesorId = asesorId;            
        }

        public void ActualizarEstado(int nuevoEstadoId)
        {
            EstadoSolicitudId = nuevoEstadoId;
            FechaActualizacion = DateTime.UtcNow;
        }
    }
}
