using System;
using System.ComponentModel.DataAnnotations;
using ExploraYa1.Destinos;

namespace ExploraYa1.Destinos
{
    public class CrearActualizarCalificacionDTO
    {
        [Required]
        public Guid DestinoTuristicoId { get; set; }

        [Required]
        public int Puntuacion { get; set; }

        [Required]
        public string? Comentario { get; set; }
    }
}