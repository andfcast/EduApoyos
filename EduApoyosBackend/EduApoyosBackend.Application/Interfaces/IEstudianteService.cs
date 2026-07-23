using EduApoyosBackend.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Application.Interfaces
{
    public interface IEstudianteService
    {
        Task<string> RegistrarEstudianteAsync(RegistroEstudianteDto dto);
        Task<IEnumerable<EstudianteDto>> ObtenerEstudiantesAsync();
        Task<string> ActualizarEstudianteAsync(EdicionEstudianteDto dto);
        Task<RespuestaPaginadaDto<EstudianteDto>> ObtenerEstudiantesPaginadosAsync(string? busqueda, int pagina, int tamanoPagina);
    }
}
