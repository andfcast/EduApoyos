using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EduApoyosBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudesController : ControllerBase
    {
        // GET: api/<SolicitudesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<SolicitudesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SolicitudesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SolicitudesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // PUT api/<SolicitudesController>/5
        [HttpPatch("{id}/estado")]
        public void Patch(int id, [FromBody] string value)
        {
        }

        //// DELETE api/<SolicitudesController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
