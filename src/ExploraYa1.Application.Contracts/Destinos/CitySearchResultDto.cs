using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExploraYa1.Destinos
{
    public class CitySearchResultDto
    {
        // ABP siempre busca "Items"
        public List<CityDto> Items { get; set; }

        // Opcional, pero recomendado: ABP suele pedir el TotalCount para armar la paginación abajo
        public long TotalCount { get; set; }
    }
}
