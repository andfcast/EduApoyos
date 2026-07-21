using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduApoyosBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiposDocumentoController : ControllerBase
    {
        private readonly ITipoDocumentoService _service;

        public TiposDocumentoController(ITipoDocumentoService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TipoDocumentoDto>))]
        public async Task<IActionResult> Get()
        {
            var result = await _service.ObtenerTiposDocumentoActivosAsync();
            return Ok(result);
        }
    }
}
