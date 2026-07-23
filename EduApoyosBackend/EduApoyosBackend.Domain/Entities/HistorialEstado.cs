using EduApoyosBackend.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Domain.Entities
{
    public class HistorialEstado : BaseEntity<Guid>
    {
        public Guid SolicitudId { get;  set; }
        public SolicitudApoyo Solicitud { get; set; } = null!;
        public int? EstadoAnteriorId { get;  set; }
        public EstadoSolicitud? EstadoAnterior { get; set; }
        public int EstadoNuevoId { get;  set; }
        public EstadoSolicitud EstadoNuevo { get; set; } = null!;
        public DateTime FechaCambio { get; set; }
        public Guid? UsuarioId { get;  set; }
        public Usuario? Usuario { get; set; }
        public string Observacion { get; set; }

        public HistorialEstado(Guid id, Guid solicitudId, int? estadoAnteriorId, int estadoNuevoId, DateTime fechaCambio, Guid? usuarioId, string observacion)
        {
            Id = id; SolicitudId = solicitudId; EstadoAnteriorId = estadoAnteriorId; EstadoNuevoId = estadoNuevoId; FechaCambio = fechaCambio; UsuarioId = usuarioId; Observacion = observacion;
        }
    }
}
