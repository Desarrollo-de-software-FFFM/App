using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace ExploraYa1.Destinos
{
    public class DestinosDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Pais, Guid> _paisRepository;
        private readonly IRepository<Region, Guid> _regionRepository;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;

        public DestinosDataSeederContributor(
            IRepository<Pais, Guid> paisRepository,
            IRepository<Region, Guid> regionRepository,
            IRepository<DestinoTuristico, Guid> destinoRepository)
        {
            _paisRepository = paisRepository;
            _regionRepository = regionRepository;
            _destinoRepository = destinoRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Primero crear el país
            var pais = await _paisRepository.InsertAsync(new Pais
            {
                Nombre = "País de Prueba"
            });

            // Luego crear la región asociada al país
            var region = await _regionRepository.InsertAsync(new Region
            {
                Nombre = "Región de Prueba",
                Descripcion = "Descripción de prueba",
                PaisId = pais.Id
            });

            // Finalmente crear el destino turístico
            await _destinoRepository.InsertAsync(new DestinoTuristico
            {
                Nombre = "Destino de Prueba",
                Latitud = 0,
                Longuitud = 0,
                Poblacion = 1000,
                CalificacionGeneral = 4,
                ImagenUrl = "test.jpg",
                RegionId = region.Id
            });
        }
    }
}