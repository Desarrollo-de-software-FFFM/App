using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace ExploraYa1.Experiencias
{
    public class Experiencia : FullAuditedAggregateRoot<Guid>
    {
        public Guid DestinoId { get; set; }
        public string Comentario { get; set; }
        public TipoValoracion Valoracion { get; set; }
        protected Experiencia() { }

        public Experiencia(Guid id, Guid destinoId, string comentario, TipoValoracion valoracion)
            : base(id)
        {
            DestinoId = destinoId;
            Comentario = comentario;
            Valoracion = valoracion;
        }
    }
}
