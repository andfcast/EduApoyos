using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Application.DTOs
{
    public class RegistroEstudianteDto
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo institucional es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener mínimo 8 caracteres.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de documento es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un tipo de documento válido.")]
        public int TipoDocumentoId { get; set; }

        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "El documento debe tener entre 5 y 20 caracteres.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "El documento solo debe contener números.")]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El programa académico es obligatorio.")]
        [StringLength(100, ErrorMessage = "El programa académico no puede superar los 100 caracteres.")]
        public string ProgramaAcademico { get; set; } = string.Empty;

        [Required(ErrorMessage = "El semestre es obligatorio.")]
        [Range(1, 12, ErrorMessage = "El semestre debe estar en un rango de 1 a 12.")]
        public int Semestre { get; set; }
    }
}
