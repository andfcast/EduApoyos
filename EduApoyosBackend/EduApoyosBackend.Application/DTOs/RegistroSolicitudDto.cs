using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Application.DTOs
{
    public class RegistroSolicitudDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El ID del estudiante es obligatorio.")]
        public Guid EstudianteId { get; set; }

        [Required(ErrorMessage = "El tipo de apoyo es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de apoyo válido.")]
        public int TipoApoyoId { get; set; }

        [Required(ErrorMessage = "El monto solicitado es obligatorio.")]
        [Range(0.01, 100_000_000.00, ErrorMessage = "El monto debe ser un valor positivo mayor a cero.")]
        [DataType(DataType.Currency)]
        public double MontoSolicitado { get; set; }

        [Required(ErrorMessage = "La descripción o justificación es obligatoria.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 1000 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

        [Range(1, 4, ErrorMessage = "El estado especificado no es válido.")]
        public int EstadoId { get; set; } = 1;

        [DataType(DataType.DateTime)]
        public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;        
        public Guid AsesorId { get; set; } = Guid.Empty;

    }
}
