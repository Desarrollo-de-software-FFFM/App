using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace ExploraYa1.Destinos
{
    public class TestMocksGeoDb
    {
        private DestinoTuristicoAppService BuildService(ICitySearchService citySearchMock)
        {
            var destinoRepo = Substitute.For<IRepository<DestinoTuristico, Guid>>();
            var paisRepo = Substitute.For<IRepository<Pais, Guid>>();
            var regionRepo = Substitute.For<IRepository<Region, Guid>>();

            return new DestinoTuristicoAppService(
                destinoRepo,
                regionRepo,
                paisRepo,
                citySearchMock
            );
        }

        [Fact]
        public async Task SearchCitiesAsync_ShouldReturnResults()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "Test" };

            var expectedCities = new List<CityDto>
            {
                new CityDto { Name = "TestCity", Country = "TestCountry" }
            };

            var citySearchMock = Substitute.For<ICitySearchService>();
            citySearchMock
                .SearchCitiesAsync(Arg.Any<CitySearchRequestDto>())
                .Returns(new CitySearchResultDto { Cities = expectedCities });

            var service = BuildService(citySearchMock);

            // Act
            var result = await service.SearchCitiesAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Cities.Count.ShouldBe(1);
            result.Cities[0].Name.ShouldBe("TestCity");
        }

        [Fact]
        public async Task SearchCitiesAsync_ReturnsEmpty()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "NoMatch" };

            var citySearchMock = Substitute.For<ICitySearchService>();
            citySearchMock
                .SearchCitiesAsync(Arg.Any<CitySearchRequestDto>())
                .Returns(new CitySearchResultDto { Cities = new List<CityDto>() });

            var service = BuildService(citySearchMock);

            // Act
            var result = await service.SearchCitiesAsync(request);

            // Assert
            result.Cities.ShouldBeEmpty();
        }

        [Fact]
        public async Task SearchCitiesAsync_InvalidInput_ReturnsEmpty()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "" };

            var citySearchMock = Substitute.For<ICitySearchService>();
            citySearchMock
                .SearchCitiesAsync(Arg.Any<CitySearchRequestDto>())
                .Returns(new CitySearchResultDto { Cities = new List<CityDto>() });

            var service = BuildService(citySearchMock);

            // Act
            var result = await service.SearchCitiesAsync(request);

            // Assert
            result.Cities.ShouldBeEmpty();
        }

        [Fact]
        public async Task SearchCitiesAsync_ApiError_ThrowsException()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "Test" };

            var citySearchMock = Substitute.For<ICitySearchService>();
            citySearchMock
                .When(x => x.SearchCitiesAsync(request))
                .Do(_ => throw new Exception("API error"));

            var service = BuildService(citySearchMock);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.SearchCitiesAsync(request));
        }
        [Fact]
        public async Task SearchCitiesAsync_FilterByCountry_ReturnsOnlyThatCountry()
        {
            var cities = new List<CityDto>
        {
            new CityDto { Name="Madrid", Country="ES" },
            new CityDto { Name="Barcelona", Country="ES" },
            new CityDto { Name="Paris", Country="FR" }
        };

            var citySearchMock = Substitute.For<ICitySearchService>();
            citySearchMock
                .SearchCitiesAsync(Arg.Any<CitySearchRequestDto>())
                .Returns(callInfo =>
                {
                    var req = callInfo.Arg<CitySearchRequestDto>();
                    var filtered = string.IsNullOrWhiteSpace(req.Country)
                        ? cities
                        : cities.FindAll(c => c.Country == req.Country);
                    return Task.FromResult(new CitySearchResultDto { Cities = filtered });
                });

            var service = BuildService(citySearchMock);
            var result = await service.SearchCitiesAsync(new CitySearchRequestDto { Country = "ES" });

            result.Cities.Count.ShouldBe(2);
            result.Cities.All(c => c.Country == "ES").ShouldBeTrue();
        }

        [Fact]
        public async Task SearchCitiesAsync_FilterByRegion_ReturnsOnlyThatRegion()
        {
            var cities = new List<CityDto>
        {
            new CityDto { Name="Madrid", Region="Madrid" },
            new CityDto { Name="Alcala", Region="Madrid" },
            new CityDto { Name="Sevilla", Region="Andalucia" }
        };

            var citySearchMock = Substitute.For<ICitySearchService>();
            citySearchMock
                .SearchCitiesAsync(Arg.Any<CitySearchRequestDto>())
                .Returns(callInfo =>
                {
                    var req = callInfo.Arg<CitySearchRequestDto>();
                    var filtered = string.IsNullOrWhiteSpace(req.Region)
                        ? cities
                        : cities.FindAll(c => c.Region == req.Region);
                    return Task.FromResult(new CitySearchResultDto { Cities = filtered });
                });

            var service = BuildService(citySearchMock);
            var result = await service.SearchCitiesAsync(new CitySearchRequestDto { Region = "Madrid" });

            result.Cities.Count.ShouldBe(2);
            result.Cities.All(c => c.Region == "Madrid").ShouldBeTrue();
        }

        [Fact]
        public async Task SearchCitiesAsync_FilterByMinimumPopulation_ReturnsOnlyCitiesAbovePopulation()
        {
            var cities = new List<CityDto>
        {
            new CityDto { Name="Madrid", Population=3300000 },
            new CityDto { Name="Barcelona", Population=1600000 },
            new CityDto { Name="Alcala", Population=200000 }
        };

            var citySearchMock = Substitute.For<ICitySearchService>();
            citySearchMock
                .SearchCitiesAsync(Arg.Any<CitySearchRequestDto>())
                .Returns(callInfo =>
                {
                    var req = callInfo.Arg<CitySearchRequestDto>();
                    var filtered = req.MinimumPopulation.HasValue
                        ? cities.FindAll(c => c.Population >= req.MinimumPopulation.Value)
                        : cities;
                    return Task.FromResult(new CitySearchResultDto { Cities = filtered });
                });

            var service = BuildService(citySearchMock);
            var result = await service.SearchCitiesAsync(new CitySearchRequestDto { MinimumPopulation = 1000000 });

            result.Cities.Count.ShouldBe(2);
            result.Cities.All(c => c.Population >= 1000000).ShouldBeTrue();
        }

        [Fact]
        public async Task SearchCitiesAsync_FilterByNameAndCountry_ReturnsMatchingCities()
        {
            var cities = new List<CityDto>
        {
            new CityDto { Name="Madrid", Country="ES" },
            new CityDto { Name="Madinat", Country="AE" },
            new CityDto { Name="Barcelona", Country="ES" }
        };

            var citySearchMock = Substitute.For<ICitySearchService>();
            citySearchMock
                .SearchCitiesAsync(Arg.Any<CitySearchRequestDto>())
                .Returns(callInfo =>
                {
                    var req = callInfo.Arg<CitySearchRequestDto>();
                    var filtered = cities.FindAll(c =>
                        (string.IsNullOrWhiteSpace(req.PartialName) || c.Name.Contains(req.PartialName)) &&
                        (string.IsNullOrWhiteSpace(req.Country) || c.Country == req.Country)
                    );
                    return Task.FromResult(new CitySearchResultDto { Cities = filtered });
                });

            var service = BuildService(citySearchMock);
            var result = await service.SearchCitiesAsync(new CitySearchRequestDto { PartialName = "Mad", Country = "ES" });

            result.Cities.Count.ShouldBe(1);
            result.Cities.All(c => c.Name.Contains("Mad") && c.Country == "ES").ShouldBeTrue();
        }
        [Fact]
        public async Task GetCityDetailsAsync_ShouldReturnCityInformation()
        {
            // Arrange
            var cityId = 123;
            var expectedCity = new CityInformationDto
            {
                Id = cityId,
                Name = "TestCity",
                Country = "TestCountry",
                Region = "TestRegion",
                Population = 100000,
                Latitude = 12.34,
                Longitude = 56.78,
                Timezone = "UTC+1"
            };

            var citySearchMock = Substitute.For<ICitySearchService>();
            citySearchMock
                .GetCityDetailsAsync(cityId)
                .Returns(expectedCity);

            var destinoRepo = Substitute.For<IRepository<DestinoTuristico, Guid>>();
            var paisRepo = Substitute.For<IRepository<Pais, Guid>>();
            var regionRepo = Substitute.For<IRepository<Region, Guid>>();

            var service = new DestinoTuristicoAppService(
                destinoRepo,
                regionRepo,
                paisRepo,
                citySearchMock
            );

            // Act
            var result = await service.GetCityDetailsAsync(cityId);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(cityId);
            result.Name.ShouldBe("TestCity");
            result.Country.ShouldBe("TestCountry");
            result.Region.ShouldBe("TestRegion");
            result.Population.ShouldBe(100000);
            result.Latitude.ShouldBe(12.34);
            result.Longitude.ShouldBe(56.78);
            result.Timezone.ShouldBe("UTC+1");
        }

        [Fact]
        public async Task GetCityDetailsAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var cityId = 999; // Id que no existe

            var citySearchMock = Substitute.For<ICitySearchService>();
            citySearchMock
                .GetCityDetailsAsync(cityId)
                .ThrowsAsync(new Exception("No se encontró información de la ciudad."));

            var destinoRepo = Substitute.For<IRepository<DestinoTuristico, Guid>>();
            var paisRepo = Substitute.For<IRepository<Pais, Guid>>();
            var regionRepo = Substitute.For<IRepository<Region, Guid>>();

            var service = new DestinoTuristicoAppService(
                destinoRepo,
                regionRepo,
                paisRepo,
                citySearchMock
            );

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetCityDetailsAsync(cityId));
        }
        [Fact]
        public async Task GetCityDetailsAsync_ApiReturnsUnsuccessfulStatusCode_ShouldThrowHttpRequestException()
        {
            var cityId = 9999;

            var citySearchMock = Substitute.For<ICitySearchService>();

            citySearchMock
                .GetCityDetailsAsync(cityId)
                .ThrowsAsync(new HttpRequestException("Response status code does not indicate success: 404 (Not Found)."));

            var destinoRepo = Substitute.For<IRepository<DestinoTuristico, Guid>>();
            var paisRepo = Substitute.For<IRepository<Pais, Guid>>();
            var regionRepo = Substitute.For<IRepository<Region, Guid>>();

            var service = new DestinoTuristicoAppService(
                destinoRepo, regionRepo, paisRepo, citySearchMock
            );

            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetCityDetailsAsync(cityId));

            await citySearchMock.Received(1).GetCityDetailsAsync(cityId);
        }

        [Fact]
        public async Task GetCityDetailsAsync_SuccessfulResponseButNoData_ShouldThrowNoDataException()
        {
            var cityId = 1000;

            CityInformationDto nullCityInfo = null;

            var citySearchMock = Substitute.For<ICitySearchService>();

            citySearchMock
                .GetCityDetailsAsync(cityId)
                .ThrowsAsync(new Exception("No se encontró información de la ciudad."));

            var destinoRepo = Substitute.For<IRepository<DestinoTuristico, Guid>>();
            var paisRepo = Substitute.For<IRepository<Pais, Guid>>();
            var regionRepo = Substitute.For<IRepository<Region, Guid>>();

            var service = new DestinoTuristicoAppService(
                destinoRepo, regionRepo, paisRepo, citySearchMock
            );

            var exception = await Assert.ThrowsAsync<Exception>(() => service.GetCityDetailsAsync(cityId));

            exception.Message.ShouldBe("No se encontró información de la ciudad.");
        }
    }
}
