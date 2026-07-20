using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public Guid UsuarioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
