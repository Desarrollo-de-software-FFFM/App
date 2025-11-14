using ExploraYa1.Destinos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos; // <-- CAMBIO: Importar DTOs de ABP

namespace ExploraYa1.DestinosTuristicos
{
    public class GeoDbCitySearchService : ICitySearchService
    {
        private readonly HttpClient _httpClient;

        public GeoDbCitySearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            // (Tu configuración de HttpClient está bien, la dejo igual)
            const string rapidApiKey = "41c717a457mshcfe32e8d4cdaf10p198265jsn4230693ac3f1";
            const string rapidApiHost = "wft-geo-db.p.rapidapi.com";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", rapidApiKey);
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", rapidApiHost);
        }

        // <-- CAMBIO: Actualizado el tipo de retorno para que coincida con la interfaz
        public async Task<PagedResultDto<CityDto>> SearchCitiesAsync(CitySearchRequestDto request)
        {
            if (string.IsNullOrEmpty(request.PartialName))
            {
                // <-- CAMBIO: Devolver un PagedResultDto vacío
                return new PagedResultDto<CityDto>(0, new List<CityDto>());
            }

            try
            {
                // <-- CAMBIO: Usar los parámetros de paginación en la URL
                // La API de GeoDb usa 'limit' (MaxResultCount) y 'offset' (SkipCount)
                var url = $"https://wft-geo-db.p.rapidapi.com/v1/geo/cities" +
                          $"?namePrefix={Uri.EscapeDataString(request.PartialName)}" +
                          $"&limit={request.MaxResultCount}" +
                          $"&offset={request.SkipCount}";

                var response = await _httpClient.GetAsync(url);

                if (response == null)
                    throw new HttpRequestException("No se pudo obtener respuesta del servidor.");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadFromJsonAsync<GeoDbResponse>();

                // <-- CAMBIO: Comprobar si hay datos o metadata
                if (json?.Data == null || json.Metadata == null)
                    return new PagedResultDto<CityDto>(0, new List<CityDto>());

                var cities = json.Data.Select(c => new CityDto
                {
                    Name = c.City ?? string.Empty,
                    Country = c.Country ?? string.Empty,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude
                }).ToList();

                // <-- CAMBIO: Devolver el PagedResultDto con el totalCount de la API
                return new PagedResultDto<CityDto>(
                    json.Metadata.TotalCount, // El conteo total de la API
                    cities                   // Los resultados de la página actual
                );
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private class GeoDbResponse
        {
            public List<GeoDbCity> Data { get; set; } = new();
            public GeoDbMetadata Metadata { get; set; } // <-- CAMBIO: Añadir Metadata
        }

        // <-- CAMBIO: Añadir clase para la Metadata de la API
        private class GeoDbMetadata
        {
            public int TotalCount { get; set; }
        }

        private class GeoDbCity
        {
            public string? City { get; set; }
            public string? Country { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
}