using ExploraYa1.Destinos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

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
            try
            {
                // Construcción dinámica de filtros
                var query = new StringBuilder("https://wft-geo-db.p.rapidapi.com/v1/geo/cities?");

                // Filtro por nombre parcial (opcional)
                if (!string.IsNullOrWhiteSpace(request.PartialName))
                    query.Append($"namePrefix={Uri.EscapeDataString(request.PartialName)}&");

                // Filtro por país
                if (!string.IsNullOrWhiteSpace(request.Country))
                    query.Append($"countryIds={Uri.EscapeDataString(request.Country)}&");

                // Filtro por región (GeoDB usa "region")
                if (!string.IsNullOrWhiteSpace(request.Region))
                    query.Append($"region={Uri.EscapeDataString(request.Region)}&");

                // Filtro por población mínima
                if (request.MinimumPopulation.HasValue)
                    query.Append($"minPopulation={request.MinimumPopulation.Value}&");

                // Límite para evitar respuestas gigantes
                query.Append("limit=10");

                var url = query.ToString();

                var response = await _httpClient.GetAsync(url);

                if (response == null)
                    throw new HttpRequestException("No se pudo obtener respuesta del servidor.");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadFromJsonAsync<GeoDbResponse>();
                if (json?.Data == null)
                    return new CitySearchResultDto { Cities = new List<CityDto>() };

                var cities = json.Data.Select(c => new CityDto
                {
                    Id = c.Id ?? 0,
                    Name = c.City ?? string.Empty,
                    Country = c.Country ?? string.Empty,
                    Region = c.Region,
                    Population = c.Population ?? 0,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude
                }).ToList();

                return new CitySearchResultDto { Cities = cities };
            }
            catch
            {
                throw;
            }

            try
            {
                // Construcción dinámica de filtros
                var query = new StringBuilder("https://wft-geo-db.p.rapidapi.com/v1/geo/cities?");
                query.Append($"namePrefix={Uri.EscapeDataString(request.PartialName)}");
                query.Append("&limit=5");

                if (!string.IsNullOrWhiteSpace(request.Country))
                    query.Append($"&countryIds={Uri.EscapeDataString(request.Country)}");

                if (!string.IsNullOrWhiteSpace(request.Region))
                    query.Append($"&regionCode={Uri.EscapeDataString(request.Region)}");

                if (request.MinimumPopulation.HasValue)
                    query.Append($"&minPopulation={request.MinimumPopulation.Value}");

                var url = query.ToString();

                var response = await _httpClient.GetAsync(url);

                if (response == null)
                    throw new HttpRequestException("No se pudo obtener respuesta del servidor.");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadFromJsonAsync<GeoDbResponse>();
                if (json?.Data == null)
                    return new CitySearchResultDto { Cities = new List<CityDto>() };

                var cities = json.Data.Select(c => new CityDto
                {
                    Id = c.Id ?? 0,
                    Name = c.City ?? string.Empty,
                    Country = c.Country ?? string.Empty,
                    Region = c.Region,
                    Population = c.Population,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude
                }).ToList();

                return new CitySearchResultDto { Cities = cities };
            }
            catch
            {
                throw; // re-lanza para que el test vea la excepción original
            }
        }

        private class GeoDbResponse
        {
            public List<GeoDbCity> Data { get; set; } = new();
        }

        private class GeoDbCity
        {
            public int? Id { get; set; }          // Nuevo: id de la ciudad
            public string? City { get; set; }
            public string? Country { get; set; }
            public string? Region { get; set; }      // Nuevo
            public int? Population { get; set; }     // Nuevo
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public async Task<CityInformationDto> GetCityDetailsAsync(int cityId)
        {
            var url = $"https://wft-geo-db.p.rapidapi.com/v1/geo/cities/{cityId}";

            var response = await _httpClient.GetAsync(url);

            if (response == null)
                throw new HttpRequestException("No se pudo obtener respuesta del servidor GeoDB.");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<GeoDbDetailResponse>();

            if (json?.Data == null)
                throw new Exception("No se encontró información de la ciudad.");

            var c = json.Data;

            return new CityInformationDto
            {
                Id = c.Id,
                Name = c.City ?? "",
                Country = c.Country ?? "",
                Region = c.Region ?? "",
                Population = c.Population ?? 0,
                Latitude = c.Latitude,
                Longitude = c.Longitude,
                Timezone = c.Timezone ?? ""
            };
        }

        private class GeoDbDetailResponse
        {
            public GeoDbDetailCity Data { get; set; } = new();
        }

        private class GeoDbDetailCity
        {
            public int Id { get; set; }
            public string? City { get; set; }
            public string? Country { get; set; }
            public string? Region { get; set; }
            public int? Population { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string? Timezone { get; set; }
        }

    }
}
