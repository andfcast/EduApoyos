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
        public string NombreCompleto { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public int RolId { get; private set; }
        public Rol Rol { get; set; } = null!;
        public DateTime FechaRegistro { get; private set; }

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
