using ExploraYa1.Destinos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ExploraYa1.DestinosTuristicos
{
    public interface IDestinoTuristicoAppService :
    
        ICrudAppService<
        DestinoTuristicoDTO,
        Guid,
        PagedAndSortedResultRequestDto,
        CrearActualizarDestinoDTO>
    {
        // Agregar el método que falta
        Task<PagedResultDto<CityDto>> SearchCitiesAsync(CitySearchRequestDto request);
    }  
}
