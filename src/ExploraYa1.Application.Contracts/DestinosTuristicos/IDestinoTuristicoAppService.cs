using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using ExploraYa1.Destinos; // Importante para reconocer CityDto y CitySearchRequestDto

namespace ExploraYa1.DestinosTuristicos
{
    public interface IDestinoTuristicoAppService : ICrudAppService<
        DestinoTuristicoDTO,
        Guid,
        PagedAndSortedResultRequestDto,
        CrearActualizarDestinoDTO>
    {
        // 👇 ESTA LÍNEA DEBE COINCIDIR EXACTAMENTE CON TU SERVICIO
        Task<PagedResultDto<CityDto>> SearchCitiesAsync(CitySearchRequestDto request);
    }
}