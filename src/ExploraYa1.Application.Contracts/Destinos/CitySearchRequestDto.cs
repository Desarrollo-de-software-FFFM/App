using Volo.Abp.Application.Dtos; // <-- ¡Importante añadir esto!

namespace ExploraYa1.Destinos
{
    // 1. Hereda de PagedAndSortedResultRequestDto
    public class CitySearchRequestDto : PagedAndSortedResultRequestDto //cambiado para el frontend
    {
        public string PartialName { get; set; }

        // 2. Añade la propiedad que faltaba
        public string Country { get; set; }
    }
}
