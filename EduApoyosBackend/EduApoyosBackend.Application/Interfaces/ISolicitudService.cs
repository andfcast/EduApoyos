using EduApoyosBackend.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Application.Interfaces
{
    public interface ISolicitudService
    {
        Task<IEnumerable<SolicitudApoyoDto>> ObtenerSolicitudesAsync();
        Task<string> RegistrarSolicitudAsync(RegistroSolicitudDto dto);
        Task<SolicitudApoyoDto> ObtenerSolicitudAsync(Guid solicitudId);
        Task<string> ActualizarEstadoSolicitudAsync(Guid solicitudId, ActualizarEstadoSolicitudDto dto);
        Task<IEnumerable<SolicitudApoyoDto>> ObtenerSolicitudesXEstudianteAsync(Guid id);
        Task<RespuestaPaginadaDto<SolicitudApoyoDto>> ObtenerSolicitudesFiltradasAsync(FiltroSolicitudDto filtro);
    }
}
