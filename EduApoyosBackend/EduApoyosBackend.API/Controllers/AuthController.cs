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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }   
        [AllowAnonymous]
        // POST api/<AuthController>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            return Ok(await _authService.Login(dto));
        }
        // POST api/<AuthController>
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistroUsuarioDto dto)
        {
            return Ok();
        }
    }
}
