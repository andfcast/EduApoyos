using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using EduApoyosBackend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduApoyosBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EstudiantesController : ControllerBase
    {
        private readonly IEstudianteService _service;
        private readonly ISolicitudService _solicitudService;
        private readonly ILogger<EstudiantesController> _logger;

        public EstudiantesController(IEstudianteService service, ISolicitudService solicitudService, ILogger<EstudiantesController> logger)
        {
            _service = service;
            _solicitudService = solicitudService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener el listado de estudiantes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Asesor")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EstudianteDto>))]
        public async Task<IActionResult> Get([FromQuery] string? busqueda, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 10) {
            var result = await _service.ObtenerEstudiantesPaginadosAsync(busqueda,pagina,tamanoPagina);
            return Ok(result);
        }
        
        /// <summary>
        /// Ingreso de estudiantes
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Asesor")]
        public async Task<IActionResult> Post([FromBody] RegistroEstudianteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _logger.LogInformation("Intentando registrar un nuevo estudiante con correo: {Email}", dto.Email);
            var resultado = await _service.RegistrarEstudianteAsync(dto);
            return Ok();
        }

        /// <summary>
        /// Actualización de datos de estudiante
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] EdicionEstudianteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _logger.LogInformation("Intentando actualizar el estudiante con ID: {Id}", dto.Id);
            var resultado = await _service.ActualizarEstudianteAsync(dto);
            return Ok();
        }

        /// <summary>
        /// Obtiene las solicitudes del estudiante
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Estudiante")]
        [HttpGet("{id:guid}/solicitudes")]
        public async Task<IActionResult> GetSolicitudes(Guid id)
        {
            var resultado = await _solicitudService.ObtenerSolicitudesXEstudianteAsync(id);
            return Ok(resultado);
        }
    }
}
