using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria.Models
{
    public class Contrato
    {
        [Key]
        [Column("id_contrato")]
        public int IdContrato { get; set; }

        [Required]
        [Column("id_inquilino")]
        public int IdInquilino { get; set; }

        [Required]
        [Column("id_inmueble")]
        public int IdInmueble { get; set; }

        [Required]
        [Column("id_usuario_creador")]
        public int IdUsuarioCreador { get; set; }

        [Column("id_usuario_finalizador")]
        public int? IdUsuarioFinalizador { get; set; }

        [Required]
        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Required]
        [Column("fecha_fin_original")]
        public DateTime FechaFinOriginal { get; set; }

        [Column("fecha_fin_anticipada")]
        public DateTime? FechaFinAnticipada { get; set; }

        [Required]
        [Column("monto_mensual")]
        public decimal MontoMensual { get; set; }

        [Column("estado")]
        public string? Estado { get; set; } // Enum: vigente, finalizado, rescindido

        [Column("multa")]
        public decimal? Multa { get; set; }
    }
}
