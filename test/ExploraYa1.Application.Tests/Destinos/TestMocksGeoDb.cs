using ExploraYa1.DestinosTuristicos;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ExploraYa1.Destinos
{
    public class TestMocksGeoDb
    {
        [Fact]
        public async Task SearchCitiesAsync_ShouldReturnResults()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "Test" };
            var expectedCities = new List<CityDto>
            {
                new CityDto
                {
                    Name = "TestCity",
                    Country = "TestCountry"
                }
            };

            var mockCitySearchService = Substitute.For<ICitySearchService>();
            mockCitySearchService
                .SearchCitiesAsync(Arg.Any<CitySearchRequestDto>())
                .Returns(Task.FromResult(new CitySearchResultDto { Cities = expectedCities }));

            // Usar un mock del repositorio para evitar dependencias de base de datos
            var repoMock = Substitute.For<IRepository<DestinoTuristico, Guid>>();
            var service = new DestinoTuristicoAppService(repoMock, mockCitySearchService);

            // Act
            var result = await service.SearchCitiesAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Cities.Count.ShouldBe(1);
            result.Cities[0].Name.ShouldBe("TestCity");
            result.Cities[0].Country.ShouldBe("TestCountry");
        }
        [Fact]
        public async Task SearchCitiesAsync_ReturnsEmpty()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "NoMatch" };
            var expected = new CitySearchResultDto { Cities = new List<CityDto>() };
            var repoMock = Substitute.For<IRepository<DestinoTuristico, Guid>>();
            var citySearchMock = Substitute.For<ICitySearchService>();
            citySearchMock.SearchCitiesAsync(request).Returns(expected);
            var service = new DestinoTuristicoAppService(repoMock, citySearchMock);

            // Act
            var result = await service.SearchCitiesAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Cities.ShouldBeEmpty();
        }

        [Fact]
        public async Task SearchCitiesAsync_InvalidInput_ReturnsEmpty()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "" };
            var expected = new CitySearchResultDto { Cities = new List<CityDto>() };
            var repoMock = Substitute.For<IRepository<DestinoTuristico, Guid>>();
            var citySearchMock = Substitute.For<ICitySearchService>();
            citySearchMock.SearchCitiesAsync(request).Returns(expected);
            var service = new DestinoTuristicoAppService(repoMock, citySearchMock);

            // Act
            var result = await service.SearchCitiesAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Cities.ShouldBeEmpty();
        }

        [Fact]
        public async Task SearchCitiesAsync_ApiError_ThrowsException()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "Test" };
            var repoMock = Substitute.For<IRepository<DestinoTuristico, Guid>>();
            var citySearchMock = Substitute.For<ICitySearchService>();
            citySearchMock
                .When(x => x.SearchCitiesAsync(request))
                .Do(x => { throw new Exception("API error"); });
            var service = new DestinoTuristicoAppService(repoMock, citySearchMock);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.SearchCitiesAsync(request));
        }
    }
}
