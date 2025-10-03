using ExploraYa1.Destinosturisticos;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;
using Xunit;
using Volo.Abp.Testing;
using Volo.Abp.Autofac;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;


namespace ExploraYa1.Destinos
{


    public abstract class DestinoTuristicoAppService_tests<TStartupModule> : ExploraYa1ApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule


    {
        private readonly IDestinosAppService _destinosAppService;

        protected DestinoTuristicoAppService_tests()
        {
            _destinosAppService = GetRequiredService<IDestinosAppService>();
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedDestinosDto()
        {
            // Arrange
            var crearDestinoDTO = new CrearActualizarDestinoDTO
            {
                Nombre = "ParqueNacional",
                Latitud = 34,
                Longuitud = 40,
                Poblacion = 500,
                CalificacionGeneral = 4,
                ImagenUrl = "asdasdasd",
                RegionId = Guid.NewGuid() // Asegúrate de usar un GUID válido

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

    }



}