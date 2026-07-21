using EduApoyosBackend.Domain.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Domain.Entities
{
    public class Usuario : BaseEntity<Guid>
    {
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;
        public DateTime FechaRegistro { get; set; }

        public Usuario(Guid id, string nombreCompleto, string email, string passwordHash, int rolId, DateTime fechaRegistro)
        {
            Id = id;
            NombreCompleto = nombreCompleto;
            Email = email;
            PasswordHash = passwordHash;
            RolId = rolId;
            FechaRegistro = fechaRegistro;
        }
    }
}
