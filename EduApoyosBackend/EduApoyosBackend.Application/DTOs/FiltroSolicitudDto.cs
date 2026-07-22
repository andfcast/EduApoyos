using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Application.DTOs
{
    public class FiltroSolicitudDto
    {
        public int? TipoApoyoId { get; set; }
        public DateTime? Fecha { get; set; }
        public int? EstadoId { get; set; }        
        public int Pagina { get; set; } = 1;
        public int TamanoPagina { get; set; } = 5;
    }
}
