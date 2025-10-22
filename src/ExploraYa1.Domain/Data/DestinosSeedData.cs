
using System;
using ExploraYa1.Destinos;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace ExploraYa1
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
            if (await _paisRepository.GetCountAsync() > 0) return;

            // Crear País
            var argentina = await _paisRepository.InsertAsync(new Pais { Nombre = "Argentina" }, autoSave: true);

            // Crear Región
            var buenosAires = await _regionRepository.InsertAsync(new Region {  Nombre = "Buenos Aires", Descripcion = "Región de prueba", PaisId = argentina.Id }, autoSave: true);

            // Crear Destino Turístico
            await _destinoRepository.InsertAsync(new DestinoTuristico
            {
                //Id = Guid.NewGuid(),
                Nombre = "Parque Nacional Iguazú",
                Latitud = 25,
                Longuitud = 54,
                Poblacion = 0,
                CalificacionGeneral = 5,
                ImagenUrl = "iguazu.jpg",
                RegionId = buenosAires.Id
            }, autoSave: true);
        }
    }
}
