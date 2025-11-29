using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
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
    }
}

