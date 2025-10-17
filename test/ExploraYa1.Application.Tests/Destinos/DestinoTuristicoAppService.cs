using ExploraYa1.DestinosTuristicos;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Autofac;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Volo.Abp.Validation;
using Xunit;
using static OpenIddict.Abstractions.OpenIddictConstants;


namespace ExploraYa1.Destinos
{


    public abstract class DestinoTuristicoAppService_tests<TStartupModule> : ExploraYa1ApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IDestinoTuristicoAppService _destinosAppService;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;
        private readonly IRepository<Region, Guid> _regionRepository;
        private readonly IRepository<Pais, Guid> _paisRepository;
        private readonly ICitySearchService _citySearchService;

        protected DestinoTuristicoAppService_tests()
        {
            _destinoRepository = GetRequiredService<IRepository<DestinoTuristico, Guid>>();
            _regionRepository = GetRequiredService<IRepository<Region, Guid>>();
            _paisRepository = GetRequiredService<IRepository<Pais, Guid>>();
            _citySearchService = GetRequiredService<ICitySearchService>();
            _destinosAppService = new DestinoTuristicoAppService(_destinoRepository, _citySearchService);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedDestinosDto()
        {
            // Arrange
            var pais = await _paisRepository.InsertAsync(new Pais
            {
                Nombre = "Argentina"
            }, autoSave: true);

            var region = await _regionRepository.InsertAsync(new Region
            {
                Nombre = "Región de test",
                Descripcion = "Para prueba",
                PaisId = pais.Id
            }, autoSave: true);

            var crearDestinoDTO = new CrearActualizarDestinoDTO
            {
                Nombre = "ParqueNacional",
                Latitud = 34,
                Longuitud = 40,
                Poblacion = 500,
                CalificacionGeneral = 4,
                ImagenUrl = "asdasdasd",
                RegionId = region.Id
            };

            // Act
            var result = await _destinosAppService.CreateAsync(crearDestinoDTO);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.Nombre.ShouldBe(crearDestinoDTO.Nombre);
            result.Poblacion.ShouldBe(crearDestinoDTO.Poblacion);
            result.CalificacionGeneral.ShouldBe(crearDestinoDTO.CalificacionGeneral);
            result.ImagenUrl.ShouldBe(crearDestinoDTO.ImagenUrl);
            result.Latitud.ShouldBe(crearDestinoDTO.Latitud);
            result.Longuitud.ShouldBe(crearDestinoDTO.Longuitud);
            result.RegionId.ShouldBe(crearDestinoDTO.RegionId);
        }

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

            // No necesitas el repositorio real para este test, puedes usar un mock si el constructor lo permite
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


    }



}