using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria.Models
{
    public class Pago
    {
        [Key]
        [Column("id_pago")]
        public int IdPago { get; set; }

        [Required]
        [Column("id_contrato")]
        public int IdContrato { get; set; }

        [Required]
        [Column("numero_pago")]
        public int NumeroPago { get; set; }

        [Required]
        [Column("fecha_pago")]
        public DateTime FechaPago { get; set; }

        [StringLength(255)]
        [Column("detalle")]
        public string? Detalle { get; set; }

        [Required]
        [Column("importe")]
        public decimal Importe { get; set; }

        [Column("estado")]
        public string? Estado { get; set; } // Enum: activo, anulado

        [Required]
        [Column("id_usuario_creador")]
        public int IdUsuarioCreador { get; set; }

        [Column("id_usuario_anulador")]
        public int? IdUsuarioAnulador { get; set; }
    }
}
