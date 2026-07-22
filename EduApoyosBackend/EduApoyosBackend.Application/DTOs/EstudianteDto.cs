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
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }        
        public int TipoDocumentoId { get; set; }
        public string TipoDocumento { get; set; }  = string.Empty;      
        public string NumeroDocumento { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int ProgramaAcademicoId { get; set; }
        public string ProgramaAcademico { get; set; } = string.Empty;
        public int Semestre { get; set; }
        public bool Activo { get; set; } = true;
    }
}
