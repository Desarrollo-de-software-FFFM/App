using System;
using Volo.Abp.Application.Dtos;

namespace ExploraYa1.Experiencias
{
    public class GetExperienciasInput : PagedAndSortedResultRequestDto
    {
        public Guid DestinoId { get; set; } 
        public TipoValoracion? Valoracion { get; set; } 
        public string? PalabrasClave { get; set; } 
    }
}
