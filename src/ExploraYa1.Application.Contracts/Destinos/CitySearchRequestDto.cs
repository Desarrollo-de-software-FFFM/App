using Volo.Abp.Application.Dtos;

namespace ExploraYa1.Destinos
{
    // 👇 AL AGREGAR ESTA HERENCIA, GANAS PAGINACIÓN AUTOMÁTICA
    public class CitySearchRequestDto : PagedAndSortedResultRequestDto
    {
        public string PartialName { get; set; }
        public string Country { get; set; }
    }
}
