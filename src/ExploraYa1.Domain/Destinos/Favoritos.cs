using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace ExploraYa1.Destinos
{
    public class Favorito : AuditedAggregateRoot<Guid>
    {
        public Guid DestinoTuristicoId { get; protected set; }
        public Guid UserId { get; protected set; }

        protected Favorito() { }

        public Favorito(Guid destinoTuristicoId, Guid userId)
        {
            DestinoTuristicoId = destinoTuristicoId;
            UserId = userId;
        }
    }
}