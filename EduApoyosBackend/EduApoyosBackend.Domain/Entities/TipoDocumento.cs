using EduApoyosBackend.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Domain.Entities
{
    public class TipoDocumento : BaseEntity<int>
    {
        public string Codigo { get; private set; }
        public string Descripcion { get; private set; }

        public TipoDocumento(int id, string codigo, string descripcion)
        {
            Id = id;
            Codigo = codigo;
            Descripcion = descripcion;
        }
    }
}
