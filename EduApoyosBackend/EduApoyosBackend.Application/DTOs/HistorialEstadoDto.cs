using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Application.DTOs
{
    public class HistorialEstadoDto
    {
        public Guid Id { get; set; }
        public Guid SolicitudId { get; set; }
        public string EstadoAnterior { get; set; } = string.Empty;
        public string EstadoSiguiente { get; set; } = string.Empty;
        public DateTime FechaCambio { get; set; }
        public string Observacion { get; set; } = string.Empty;
    }
}
