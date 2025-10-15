using ExploraYa1.Destinos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace ExploraYa1.DestinosTuristicos
{
    public class GeoDbCitySearchService : ICitySearchService
    {
        private readonly HttpClient _httpClient;

        public GeoDbCitySearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            const string rapidApiKey = "41c717a457mshcfe32e8d4cdaf10p198265jsn4230693ac3f1";
            const string rapidApiHost = "wft-geo-db.p.rapidapi.com";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", rapidApiKey);
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", rapidApiHost);
        }

        public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
        {
            if (string.IsNullOrEmpty(request.PartialName))
            {
                return new CitySearchResultDto { Cities = new List<CityDto>() };
            }

            try
            {
                var url = $"https://wft-geo-db.p.rapidapi.com/v1/geo/cities?namePrefix={Uri.EscapeDataString(request.PartialName)}&limit=5";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadFromJsonAsync<GeoDbResponse>();
                if (json?.Data == null)
                    return new CitySearchResultDto { Cities = new List<CityDto>() };

                var cities = json.Data.Select(c => new CityDto
                {
                    Name = c.City ?? string.Empty,
                    Country = c.Country ?? string.Empty,
                    //Latitude = c.Latitude,
                    //Longitude = c.Longitude

                }).ToList();

                return new CitySearchResultDto { Cities = cities };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar ciudades: {ex.Message}");
            }
        }

        private class GeoDbResponse
        {
            public List<GeoDbCity> Data { get; set; } = new();
        }

        private class GeoDbCity
        {
            public string? City { get; set; }
            public string? Country { get; set; }
            //public double Latitude { get; set; }
            //public double Longitude { get; set; }
        }
    }
}