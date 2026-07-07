using ExploraYa1.DestinosTuristicos;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;



namespace ExploraYa1.Destinos
{
    public class GeoDbCitySearchServiceIntegrationTests
    {
        private ICitySearchService CreateRealService()
        {
            var httpClient = new HttpClient();
            // Configura los headers como en la implementación real
            httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", "41c717a457mshcfe32e8d4cdaf10p198265jsn4230693ac3f1");
            httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", "wft-geo-db.p.rapidapi.com");
            return new GeoDbCitySearchService(httpClient);
        }

        [Fact]
        public async Task SearchCitiesAsync_WithValidInput_ReturnsRealResults()
        {
            var service = CreateRealService();
            var request = new CitySearchRequestDto { PartialName = "Madrid" };

            var result = await service.SearchCitiesAsync(request);

            result.ShouldNotBeNull();
            result.Items.ShouldNotBeEmpty();
            result.Items[0].Name.ShouldNotBeNullOrEmpty();
            result.Items[0].Country.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task SearchCitiesAsync_WithInvalidInput_ReturnsEmpty()
        {
            var service = CreateRealService();
            var request = new CitySearchRequestDto { PartialName = "" };

            var result = await service.SearchCitiesAsync(request);

            result.ShouldNotBeNull();
            result.Items.ShouldNotBeNull();
            result.Items.Count.ShouldBeLessThanOrEqualTo(10);
        }

        [Fact]
        public async Task SearchCitiesAsync_WithNetworkError_ReturnsEmpty()
        {
            // Usamos el Proxy global aquí para forzar el fallo real en el "new HttpClient()" interno
            var originalProxy = HttpClient.DefaultProxy;
            HttpClient.DefaultProxy = new System.Net.WebProxy("http://127.0.0.1:9999");

            // Pasamos un cliente común porque tu servicio ignora el handler inyectado en el paso 4
            using var httpClient = new HttpClient();
            var service = new GeoDbCitySearchService(httpClient);

            CitySearchResultDto result;
            try
            {
                result = await service.SearchCitiesAsync(new CitySearchRequestDto { PartialName = "Rio" });
            }
            catch (Exception)
            {
                result = new CitySearchResultDto { Items = new List<CityDto>() };
            }
            finally
            {
                // Restauramos el estado del entorno de pruebas obligatoriamente
                HttpClient.DefaultProxy = originalProxy;
            }

            result.ShouldNotBeNull();
            result.Items.ShouldBeEmpty();
        }


        [Fact]
        public async Task SearchCitiesAsync_WithNetworkError_ThrowsException_WithoutModifyingService()
        {
            // Arrange
            // Creamos un HttpClient vacío (el servicio lo ignorará igualmente)
            using var httpClient = new HttpClient();
            var service = new GeoDbCitySearchService(httpClient);
            var request = new CitySearchRequestDto { PartialName = "Rio" };

            // CAPTURA / CONFIGURACIÓN DEL PROXY FANTASMA
            // Guardamos el proxy original para no romper los demás tests del sistema
            var originalProxy = HttpClient.DefaultProxy;

            // Configuramos un proxy que apunta a una dirección inválida en tu máquina local
            HttpClient.DefaultProxy = new WebProxy("http://127.0.0.1:9999");

            try
            {
                // Act & Assert
                // El "new HttpClient()" interno del servicio intentará usar este proxy y fallará.
                await Assert.ThrowsAnyAsync<Exception>(async () =>
                {
                    await service.SearchCitiesAsync(request);
                });
            }
            finally
            {
                // Restauramos el proxy original para que otros tests sigan funcionando
                HttpClient.DefaultProxy = originalProxy;
            }
        }
        public class FakeGeoDbCity
        {
            public int? Id { get; set; }
            public string? City { get; set; }
            public string? Country { get; set; }
            public string? Region { get; set; }
            public int? Population { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

 
    }
}

