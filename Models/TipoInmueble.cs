using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria.Models
{
    public class TipoInmueble
    {
        [Key]
        [Column("id_tipo")]
        public int IdTipo { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        public override string ToString()
        {
            return $"IdTipo: {IdTipo}, " +
                   $"Nombre: {Nombre}, " +
                   $"Descripcion: {Descripcion}, " +
                   $"Activo: {Activo}";
        }

    }
}
