using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduApoyosBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {        

        // POST api/<AuthController>
        [HttpPost("Login")]
        public void Login([FromBody] string value)
        {
        }
        // POST api/<AuthController>
        [HttpPost("Register")]
        public void Register([FromBody] string value)
        {
        }        
    }
}
