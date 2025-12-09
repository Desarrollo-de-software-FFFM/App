using System;
using ExploraYa1.Destinos;

namespace ExploraYa1.Destinos
{
    public class CalificacionDto
    {
        public Guid Id { get; set; }
        public Guid DestinoTuristicoId { get; set; }
        public Guid UserId { get; set; }
        public int Puntuacion { get; set; }
        public string? Comentario { get; set; }
        public DateTime CreationTime { get; set; }
    }
}