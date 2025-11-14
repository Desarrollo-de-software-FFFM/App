using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace ExploraYa1.Destinos
{
    public interface ICitySearchService
    {
        Task<PagedResultDto<CityDto>> SearchCitiesAsync(CitySearchRequestDto request);
    }

}