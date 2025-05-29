using System.ComponentModel.DataAnnotations;

namespace ProyectoNominas.API.Domain.Entities
{
    public class Puesto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del puesto es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; }

        [StringLength(250, ErrorMessage = "La descripción no puede tener más de 250 caracteres")]
        public string? Descripcion { get; set; }
    }
}