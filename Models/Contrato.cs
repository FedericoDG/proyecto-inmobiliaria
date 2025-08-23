using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria.Models
{
    public class Contrato
    {
        [Key]
        [Column("id_contrato")]
        public int IdContrato { get; set; }

        [Required(ErrorMessage = "El inquilino es obligatorio")]
        [Column("id_inquilino")]
        public int? IdInquilino { get; set; }

        [Required(ErrorMessage = "El inmueble es obligatorio")]
        [Column("id_inmueble")]
        public int? IdInmueble { get; set; }

        [Required(ErrorMessage = "El usuario creador es obligatorio")]
        [Column("id_usuario_creador")]
        public int IdUsuarioCreador { get; set; }

        [Column("id_usuario_finalizador")]
        public int? IdUsuarioFinalizador { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de finalización original es obligatoria")]
        [Column("fecha_fin_original")]
        public DateTime FechaFinOriginal { get; set; }

        [Column("fecha_fin_anticipada")]
        public DateTime? FechaFinAnticipada { get; set; }

        [Required(ErrorMessage = "El monto mensual es obligatorio")]
        [Column("monto_mensual")]
        public decimal? MontoMensual { get; set; }

        [Column("estado")]
        public string? Estado { get; set; } // Enum: vigente, finalizado, rescindido

        [Column("multa")]
        public decimal? Multa { get; set; }
    }
}
