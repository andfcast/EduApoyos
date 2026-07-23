using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduApoyosBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, IUsuarioService usuarioService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _usuarioService = usuarioService;
            _logger = logger;
        }

        /// <summary>
        /// Login del usuario
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        // POST api/<AuthController>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _logger.LogInformation("Intento de inicio de sesión para el correo: {Email}", dto.Email);
            var respuesta = await _authService.LoginAsync(dto);
            return Ok(respuesta);

        }
        // POST api/<AuthController>
        /// <summary>
        /// Registrar nuevo usuario
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistroUsuarioDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _logger.LogInformation("Intentando registrar un nuevo usuario con correo: {Email}", dto.Email);
            var resultado = await _usuarioService.RegistrarUsuarioAsync(dto);
            return Ok();
        }
    }
}
