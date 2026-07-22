using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using EduApoyosBackend.Application.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduApoyosBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EstudianteDto>))]
        public async Task<IActionResult> Get()
        {
            var result = await _service.ObtenerEstudiantesAsync();
            return Ok(result);
        }
        
        // POST api/<EstudiantesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegistroEstudianteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _logger.LogInformation("Intentando registrar un nuevo estudiante con correo: {Email}", dto.Email);
            var resultado = await _service.RegistrarEstudianteAsync(dto);
            return Ok();
        }

        // PUT api/<EstudiantesController>/5
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] EdicionEstudianteDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _logger.LogInformation("Intentando actualizar el estudiante con ID: {Id}", dto.Id);
            var resultado = await _service.ActualizarEstudianteAsync(dto);
            return Ok();
        }

        // GET api/<EstudiantesController>/id/solicitudes
        [HttpGet("{id:guid}/solicitudes")]
        public async Task<IActionResult> GetSolicitudes(Guid id)
        {
            var resultado = await _solicitudService.ObtenerSolicitudesXEstudianteAsync(id);
            return Ok(resultado);
        }
    }
}
