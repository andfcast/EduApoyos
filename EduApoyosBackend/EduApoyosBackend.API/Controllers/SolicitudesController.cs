using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduApoyosBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SolicitudesController : ControllerBase
    {
        private readonly ISolicitudService _service;
        private readonly ILogger<SolicitudesController> _logger;

        public SolicitudesController(ISolicitudService service, ILogger<SolicitudesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SolicitudApoyoDto>))]
        //public async Task<IActionResult> Get()
        //{
        //    _logger.LogInformation("Intentando obtener las solicitudes");
        //    var result = await _service.ObtenerSolicitudesAsync();
        //    return Ok(result);
        //}

        /// <summary>
        /// GET /api/solicitudes
        /// Parámetros en query string: tipoApoyoId, fecha, estadoId, pagina, tamanoPagina
        /// Ejemplo: /api/solicitudes?tipoApoyoId=1&estadoId=2&pagina=1&tamanoPagina=5
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Asesor")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RespuestaPaginadaDto<SolicitudApoyoDto>))]
        public async Task<ActionResult<RespuestaPaginadaDto<SolicitudApoyoDto>>> GetSolicitudes([FromQuery] FiltroSolicitudDto filtro)
        {
            _logger.LogInformation("Intentando obtener las solicitudes");
            var resultado = await _service.ObtenerSolicitudesFiltradasAsync(filtro);
            return Ok(resultado);
        }

        // POST api/<SolicitudesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegistroSolicitudDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _logger.LogInformation("Intentando registrar un nueva solicitud");
            var resultado = await _service.RegistrarSolicitudAsync(dto);
            return NoContent();
        }

        [Authorize(Roles = "Asesor")]
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] ActualizarEstadoSolicitudDto dto)
        {
            var resultado = await _service.ActualizarEstadoSolicitudAsync(id, dto);
            _logger.LogInformation("Intentando actualizar el estado de una solicitud" + id.ToString());
            return NoContent();
        }
    }
}
