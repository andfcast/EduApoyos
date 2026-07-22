using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduApoyosBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadosSolicitudController : ControllerBase
    {
        private readonly IEstadoSolicitudService _service;

        public EstadosSolicitudController(IEstadoSolicitudService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EstadoSolicitudDto>))]
        public async Task<IActionResult> Get()
        {
            var result = await _service.ObtenerEstadosSolicitudAsync();
            return Ok(result);
        }
    }
}
