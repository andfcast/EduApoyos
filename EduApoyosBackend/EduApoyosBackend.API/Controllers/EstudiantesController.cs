using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduApoyosBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstudiantesController : ControllerBase
    {
        // GET: api/<EstudiantesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<EstudiantesController>/5
        [HttpGet("{id}")]
        public string Get(Guid id)
        {
            return "value";
        }

        // POST api/<EstudiantesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // GET api/<EstudiantesController>/5
        [HttpGet("{id}/solicitudes")]
        public string GetSolicitudes(int id)
        {
            return "value";
        }
    }
}
