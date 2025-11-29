using System;
using System.ComponentModel.DataAnnotations;

namespace ExploraYa1.Experiencias
{
    public class CrearActualizarExperienciaDto
    {
        [Required]
        public Guid DestinoId { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "El comentario no puede exceder los 1000 caracteres.")]
        public string Comentario { get; set; }

        [Required]
        public TipoValoracion Valoracion { get; set; }
    }
}
