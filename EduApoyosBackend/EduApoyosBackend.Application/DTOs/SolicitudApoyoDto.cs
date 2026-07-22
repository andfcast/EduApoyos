using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Application.DTOs
{
    public class SolicitudApoyoDto
    {
        public Guid Id { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string TipoApoyo { get; set; } = string.Empty;
        public double MontoSolicitado { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string NombreAsesor { get; set; } = string.Empty;
        public List<HistorialEstadoDto> HistorialEstados { get; set; } = new List<HistorialEstadoDto>();
    }
}
