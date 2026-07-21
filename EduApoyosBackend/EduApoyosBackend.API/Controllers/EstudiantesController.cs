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
        private readonly ILogger<EstudiantesController> _logger;

        public EstudiantesController(IEstudianteService service, ILogger<EstudiantesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EstudianteDto>))]
        public async Task<IActionResult> Get()
        {
            var result = await _service.ObtenerEstudiantesAsync();
            return Ok(result);
        }

        // GET api/<EstudiantesController>/5
        [HttpGet("{id}")]
        public string Get(Guid id)
        {
            return "value";
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

        // GET api/<EstudiantesController>/5
        [HttpGet("{id}/solicitudes")]
        public string GetSolicitudes(int id)
        {
            return "value";
        }
    }
}
