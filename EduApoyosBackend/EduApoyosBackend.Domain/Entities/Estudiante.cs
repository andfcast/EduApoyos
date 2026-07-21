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
        public Guid UsuarioId { get; private set; }
        public Usuario Usuario { get; private set; } = null!;
        public int TipoDocumentoId { get; private set; }
        public TipoDocumento TipoDocumento { get; private set; } = null!;
        public string NumeroDocumento { get; private set; }
        public string ProgramaAcademico { get; private set; }
        public int Semestre { get; private set; }
        public bool Activo { get; set; } = true;
        public ICollection<SolicitudApoyo> Solicitudes { get; private set; } = new List<SolicitudApoyo>();

        public Estudiante(Guid id, Guid usuarioId, int tipoDocumentoId, string numeroDocumento, string programaAcademico, int semestre)
        {
            Id = id; UsuarioId = usuarioId; TipoDocumentoId = tipoDocumentoId; NumeroDocumento = numeroDocumento; ProgramaAcademico = programaAcademico; Semestre = semestre;
        }
    }
}
