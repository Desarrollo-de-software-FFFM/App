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
            _destinosAppService = GetRequiredService<IDestinoTuristicoAppService>();
        }

        [Fact]
        public async Task Should_Create_Destino_Successfully()
        {
            // Arrange: crear país
            var pais = await _paisRepository.InsertAsync(
                new Pais { Nombre = "Argentina" },
                autoSave: true
            );

            // Arrange: crear región
            var region = await _regionRepository.InsertAsync(
                new Region
                {
                    Nombre = "Región test",
                    Descripcion = "Descripción prueba",
                    PaisId = pais.Id
                },
                autoSave: true
            );

            // DTO a crear
            var input = new CrearActualizarDestinoDTO
            {
                Nombre = "Parque Nacional",
                Poblacion = 500,
                Latitud = 34,
                Longuitud = 40,
                ImagenUrl = "test.png",
                CalificacionGeneral = 4,
                RegionId = region.Id
            };

            // Act
            var result = await _destinosAppService.CreateAsync(input);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.Nombre.ShouldBe("Parque Nacional");
            result.RegionId.ShouldBe(region.Id);

            // Verificar persistencia real
            var saved = await _destinoRepository.GetAsync(result.Id);
            saved.ShouldNotBeNull();
            saved.Nombre.ShouldBe(input.Nombre);
        }
    }
}




