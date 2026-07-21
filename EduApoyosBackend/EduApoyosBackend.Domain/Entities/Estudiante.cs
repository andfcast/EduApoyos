using EduApoyosBackend.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Domain.Entities
{
    public class Estudiante : BaseEntity<Guid>
    {
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public int TipoDocumentoId { get;  set; }
        public TipoDocumento TipoDocumento { get;  set; } = null!;
        public string NumeroDocumento { get;  set; }
        public int ProgramaAcademicoId { get; set; }
        public ProgramaAcademico ProgramaAcademico { get; set; } = null!;
        public int Semestre { get; set; }
        public bool Activo { get; set; } = true;
        public ICollection<SolicitudApoyo> Solicitudes { get; set; } = new List<SolicitudApoyo>();

        public Estudiante(Guid id, Guid usuarioId, int tipoDocumentoId, string numeroDocumento, int programaAcademicoId, int semestre)
        {
            Id = id; UsuarioId = usuarioId; TipoDocumentoId = tipoDocumentoId; NumeroDocumento = numeroDocumento; ProgramaAcademicoId = programaAcademicoId; Semestre = semestre;
        }
    }
}
