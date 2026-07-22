using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduApoyosBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudesController : ControllerBase
    {
        private readonly ISolicitudService _service;
        private readonly ILogger<SolicitudesController> _logger;

        public SolicitudesController(ISolicitudService service, ILogger<SolicitudesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SolicitudApoyoDto>))]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Intentando obtener las solicitudes");
            var result = await _service.ObtenerSolicitudesAsync();
            return Ok(result);
        }

        // GET api/<SolicitudesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _service.ObtenerSolicitudAsync(id);
            return Ok(result);
        }

        // POST api/<SolicitudesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegistroSolicitudDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _logger.LogInformation("Intentando registrar un nueva solicitud");
            var resultado = await _service.RegistrarSolicitudAsync(dto);
            return Ok();
        }
                
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> Patch(Guid id)
        {
            var resultado = await _service.ActualizarEstadoSolicitudAsync(id);
            _logger.LogInformation("Intentando actualizar el estado de una solicitud" + id.ToString());
            return Ok(resultado);
        }
    }
}
