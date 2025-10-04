using System;
using ExploraYa1.DestinosTuristicos;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Autofac;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Volo.Abp.Validation;
using Xunit;


namespace ExploraYa1.Destinos
{


    public abstract class DestinoTuristicoAppService_tests<TStartupModule> : ExploraYa1ApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule


    {
        private readonly IDestinoTuristicoAppService _destinosAppService;

        protected DestinoTuristicoAppService_tests() => _destinosAppService = GetRequiredService<IDestinoTuristicoAppService>();

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedDestinosDto()
        {

            var paisRepo = GetRequiredService<IRepository<Pais, Guid>>();
            var pais1 = await paisRepo.InsertAsync(new Pais
            {
                Nombre = "Argentina",
                 
            }, autoSave: true);

            var regionRepo = GetRequiredService<IRepository<Region, Guid>>();
            var region1 = await regionRepo.InsertAsync(new Region
            {
                Nombre = "Región de test",
                Descripcion = "Para prueba",
                PaisId = pais1.Id // o un país que hayas creado
            }, autoSave: true);


            var allRegiones = await regionRepo.GetListAsync();
            var allPaises = await paisRepo.GetListAsync();

            allPaises.Count.ShouldBe(1);
            allRegiones.Count.ShouldBe(1);

            // Arrange
            var crearDestinoDTO = new CrearActualizarDestinoDTO
            {
                Nombre = "ParqueNacional",
                Latitud = 34,
                Longuitud = 40,
                Poblacion = 500,
                CalificacionGeneral = 4,
                ImagenUrl = "asdasdasd",
                RegionId = region1.Id
                


                // Asegúrate de usar un GUID válido

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