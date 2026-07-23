using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Application.DTOs
{
    public class RespuestaPaginadaDto<T>
    {
        public List<T> Elementos { get; set; } = new();
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
    }
}
