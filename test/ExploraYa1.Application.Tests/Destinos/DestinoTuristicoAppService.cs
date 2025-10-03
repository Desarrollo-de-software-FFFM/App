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
                Nombre = "Parque Nacional",
                Latitud = 34,
                Longuitud = -118,
                Poblacion = 500,
                CalificacionGeneral = 5,
                ImagenUrl = "asdasdasd",
                RegionId = new Guid("22222222 - 2222 - 2222 - 2222 - 222222222222") // Asegúrate de usar un GUID válido
            };
            
            // Act
            var createdDestino = await _destinosAppService.CreateAsync(crearDestinoDTO);
            
            
            
            // Assert
            createdDestino.ShouldNotBeNull();
            createdDestino.Id.ShouldNotBe(Guid.Empty);
            createdDestino.Nombre.ShouldBe(crearDestinoDTO.Nombre);
            createdDestino.Poblacion.ShouldBe(crearDestinoDTO.Poblacion);
            createdDestino.CalificacionGeneral.ShouldBe(crearDestinoDTO.CalificacionGeneral);
            createdDestino.ImagenUrl.ShouldBe(crearDestinoDTO.ImagenUrl);
            createdDestino.Latitud.ShouldBe(crearDestinoDTO.Latitud);
            createdDestino.Longuitud.ShouldBe(crearDestinoDTO.Longuitud);
            createdDestino.RegionId.ShouldBe(crearDestinoDTO.RegionId);
        }

    }






}
