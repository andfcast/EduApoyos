using EduApoyosBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Application.DTOs
{
    public class EstudianteDto
    {
        public Guid UsuarioId { get; private set; }        
        public int TipoDocumentoId { get; private set; }        
        public string NumeroDocumento { get; private set; } = string.Empty;
        public string ProgramaAcademico { get; private set; } = string.Empty;
        public int Semestre { get; private set; }
        public bool Activo { get; set; } = true;
    }
}
