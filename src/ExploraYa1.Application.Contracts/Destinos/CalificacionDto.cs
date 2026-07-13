using System;
using ExploraYa1.Destinos;

namespace ExploraYa1.Destinos
{
    public class CalificacionDto
    {
        public Guid Id { get; set; }
        public Guid DestinoTuristicoId { get; set; }
        public string DestinoNombre { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Puntuacion { get; set; }
        public string Comentario { get; set; }
        public DateTime CreationTime { get; set; }
    }
}