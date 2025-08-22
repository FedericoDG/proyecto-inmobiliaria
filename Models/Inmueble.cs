using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria.Models
{
    public class Inmueble
    {
        [Key]
        [Column("id_inmueble")]
        public int IdInmueble { get; set; }

        [Required(ErrorMessage = "El ID del propietario es obligatorio")]
        [Column("id_propietario")]
        public int? IdPropietario { get; set; }

        [Required(ErrorMessage = "El ID del tipo es obligatorio")]
        [Column("id_tipo")]
        public int? IdTipo { get; set; }

        [Required(ErrorMessage = "El uso es obligatorio")]
        [Column("uso")]
        public string Uso { get; set; } = string.Empty; // Enum: residencial, comercial

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(255)]
        [Column("direccion")]
        public string Direccion { get; set; } = string.Empty;

        [Column("cantidad_ambientes")]
        public int? CantidadAmbientes { get; set; }

        [StringLength(100)]
        [Column("coordenadas")]
        public string? Coordenadas { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Column("precio")]
        public decimal? Precio { get; set; }

        [Column("estado")]
        public string? Estado { get; set; } // Enum: disponible, suspendido, ocupado

        [Column("activo")]
        public bool Activo { get; set; } = true;
    }
}
