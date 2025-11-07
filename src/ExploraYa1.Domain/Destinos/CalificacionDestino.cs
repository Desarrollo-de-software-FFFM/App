using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace ExploraYa1.Destinos
{
    public class CalificacionDestino : AuditedAggregateRoot<Guid>, IUserOwned
    {
        public Guid DestinoTuristicoId { get; set; }
        public Guid UserId { get; set; }
        public int Puntuacion { get; set; }
        public string Comentario { get; set; }
    }

    public interface IUserOwned
    {
        Guid UserId { get; set; }
    }
}