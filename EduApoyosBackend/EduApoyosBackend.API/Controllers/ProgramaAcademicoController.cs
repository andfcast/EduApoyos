using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduApoyosBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProgramaAcademicoController : ControllerBase
    {
        private readonly IProgramaAcademicoService _service;

        public ProgramaAcademicoController(IProgramaAcademicoService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProgramaAcademicoDto>))]
        public async Task<IActionResult> Get()
        {
            var result = await _service.ObtenerProgramasAcademicosActivosAsync();
            return Ok(result);
        }
    }
}
