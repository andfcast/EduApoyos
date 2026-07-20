using EduApoyosBackend.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Domain.Entities
{
    public class EstadoSolicitud : BaseEntity<int>
    {
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }

        public EstadoSolicitud(int id, string nombre, string descripcion)
        {
            Id = id;
            Nombre = nombre;
            Descripcion = descripcion;
        }
    }
}
