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
        public Guid SolicitudId { get; private set; }
        public SolicitudApoyo Solicitud { get; private set; } = null!;
        public int? EstadoAnteriorId { get; private set; }
        public EstadoSolicitud? EstadoAnterior { get; private set; }
        public int EstadoNuevoId { get; private set; }
        public EstadoSolicitud EstadoNuevo { get; private set; } = null!;
        public DateTime FechaCambio { get; private set; }
        public Guid UsuarioId { get; private set; }
        public Usuario Usuario { get; private set; } = null!;
        public string Observacion { get; private set; }

        public HistorialEstado(Guid id, Guid solicitudId, int? estadoAnteriorId, int estadoNuevoId, DateTime fechaCambio, Guid usuarioId, string observacion)
        {
            Id = id; SolicitudId = solicitudId; EstadoAnteriorId = estadoAnteriorId; EstadoNuevoId = estadoNuevoId; FechaCambio = fechaCambio; UsuarioId = usuarioId; Observacion = observacion;
        }
    }
}
