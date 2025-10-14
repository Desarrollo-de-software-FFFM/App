﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExploraYa1.Destinos
{
    public interface ICitySearchService
    {
        Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request);
    }

}