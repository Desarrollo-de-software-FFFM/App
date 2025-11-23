using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;

namespace ExploraYa1.DestinosTuristicos
{
    public class DestinoTuristicoAppService :
    CrudAppService<
        DestinoTuristico,
        DestinoTuristicoDTO,
        Guid,
        PagedAndSortedResultRequestDto,
        CrearActualizarDestinoDTO>,
    IDestinoTuristicoAppService
    {
        private readonly ICitySearchService _citySearchService;

        public DestinoTuristicoAppService(IRepository<DestinoTuristico, Guid> repository, ICitySearchService citySearchService)
            : base(repository)
        {
            _citySearchService = citySearchService;
        }

        // 👇 CAMBIO IMPORTANTE AQUÍ 👇
        // Cambiamos el tipo de retorno a PagedResultDto<CityDto>
        public async Task<PagedResultDto<CityDto>> SearchCitiesAsync(CitySearchRequestDto request)
        {
            // 1. Obtenemos los resultados del servicio externo
            var result = await _citySearchService.SearchCitiesAsync(request);

            // 2. Convertimos (mapeamos) la respuesta al formato estándar de ABP
            // Usamos result.Cities.Count como total (o 0 si es nulo)
            // y devolvemos la lista de ciudades.
            return new PagedResultDto<CityDto>(
                result.Cities?.Count ?? 0,
                result.Cities ?? new List<CityDto>()
            );
        }
    }
}




