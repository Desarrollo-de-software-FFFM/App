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
                // 1. Empezamos con la base correcta
                var urlBase = "https://wft-geo-db.p.rapidapi.com/v1/geo/cities";

                // 2. Guardamos los filtros que realmente vienen con datos
                var parametros = new List<string>();

                if (!string.IsNullOrWhiteSpace(request.PartialName))
                    parametros.Add($"namePrefix={Uri.EscapeDataString(request.PartialName)}");

                if (!string.IsNullOrWhiteSpace(request.Country))
                    parametros.Add($"countryIds={Uri.EscapeDataString(request.Country)}");

                if (!string.IsNullOrWhiteSpace(request.Region))
                    parametros.Add($"regionCode={Uri.EscapeDataString(request.Region)}"); // Usamos regionCode como en tu segundo try

                if (request.MinimumPopulation.HasValue)
                    parametros.Add($"minPopulation={request.MinimumPopulation.Value}");

                // Límite fijo de seguridad
                parametros.Add("limit=10");

                // 3. Unimos todo de forma segura (automáticamente pone los "&" en el medio)
                var urlFinal = $"{urlBase}?{string.Join("&", parametros)}";

                // 4. Hacemos la petición con un cliente 100% limpio
                using var tempClient = new HttpClient();
                tempClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", "41c717a457mshcfe32e8d4cdaf10p198265jsn4230693ac3f1");
                tempClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", "wft-geo-db.p.rapidapi.com");

                var response = await tempClient.GetAsync(urlFinal);

                // LA MAGIA: Si falla, extraemos el secreto que nos manda GeoDB
                if (!response.IsSuccessStatusCode)
                {
                    var errorReal = await response.Content.ReadAsStringAsync();
                    throw new Exception($"\n--- ERROR DE LA API ---\nCódigo: {response.StatusCode}\nURL Exacta: {urlFinal}\nRespuesta del servidor: {errorReal}\n-----------------------");
                }

                // Si falla, te mostrará exactamente qué código de error devolvió GeoDB
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error de la API GeoDB. Código de estado: {response.StatusCode}");

                // 5. Mapeamos la respuesta
                var json = await response.Content.ReadFromJsonAsync<GeoDbResponse>();

                if (json?.Data == null)
                    return new CitySearchResultDto { Items = new List<CityDto>() };

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

                return new CitySearchResultDto { Items = cities };
            }
            catch
            {
                throw;
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
