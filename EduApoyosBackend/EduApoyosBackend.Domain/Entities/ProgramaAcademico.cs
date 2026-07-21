using EduApoyosBackend.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Domain.Entities
{
    public class ProgramaAcademico : BaseEntity<int>
    {
        public string Descripcion { get; private set; } = string.Empty;
        
        public ProgramaAcademico(int id, string descripcion)
        {
            Id = id;
            Descripcion = descripcion;
        }
    }
}
