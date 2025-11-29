using System;
using Volo.Abp.Application.Dtos;

namespace ExploraYa1.Experiencias
{
    public class ExperienciaDto : AuditedEntityDto<Guid>
    {
        public Guid DestinoId { get; set; }
        public string Comentario { get; set; }
        public TipoValoracion Valoracion { get; set; }
    }
}
