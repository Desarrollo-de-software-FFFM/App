using System;

namespace ExploraYa1.Destinos
{
    public class FavoritoDto
    {
        public Guid Id { get; set; }
        public Guid DestinoTuristicoId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreationTime { get; set; }
    }
}