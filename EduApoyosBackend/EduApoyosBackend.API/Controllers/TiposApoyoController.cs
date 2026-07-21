using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduApoyosBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiposApoyoController : ControllerBase
    {
        private readonly ITipoApoyoService _service;

        public TiposApoyoController(ITipoApoyoService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TipoApoyoDto>))]
        public async Task<IActionResult> Get()
        {
            var result = await _service.ObtenerTiposApoyoAsync();
            return Ok(result);
        }
    }
}
