using EduApoyosBackend.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EduApoyosBackend.Domain.Entities
{
    public class Rol : BaseEntity<int>
    {
        public string Nombre { get; private set; }

        public Rol(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }
    }
}
