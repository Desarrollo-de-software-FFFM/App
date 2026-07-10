using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ExploraYa1.DestinosTuristicos
{
    public class DestinoTuristicoAppService :
    CrudAppService<
        DestinoTuristico,
        DestinoTuristicoDTO,
        Guid,
        PagedAndSortedResultRequestDto,
        CrearActualizarDestinoDTO>,
    IDestinoTuristicoAppService
    {
       
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;
        private readonly IRepository<Region, Guid> _regionRepository;
        private readonly IRepository<Pais, Guid> _paisRepository;
        private readonly ICitySearchService _citySearchService;
        private ICitySearchService citySearchMock;
        private IRepository<Pais, Guid> paisRepo;
        private IRepository<Region, Guid> regionRepo;
        private ICitySearchService citySearchMock1;
        private IRepository<Pais, Guid> paisRepo1;
        private IRepository<Region, Guid> regionRepo1;

        public DestinoTuristicoAppService(
            IRepository<DestinoTuristico, Guid> destinoRepository,
            IRepository<Region, Guid> regionRepository,
            IRepository<Pais, Guid> paisRepository,
            ICitySearchService citySearchService)
            : base(destinoRepository)
        {
            _destinoRepository = destinoRepository;
            _regionRepository = regionRepository;
            _paisRepository = paisRepository;
            _citySearchService = citySearchService;
        }


        public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
        {
            return await _citySearchService.SearchCitiesAsync(request);
        }

        public async Task<CityInformationDto> GetCityDetailsAsync(int id)
        {
            return await _citySearchService.GetCityDetailsAsync(id);
        }

        public async Task<DestinoTuristicoDTO> SyncDestinoLocalAsync(CityInformationDto city)
        {
            if (city == null || string.IsNullOrWhiteSpace(city.Name))
                throw new UserFriendlyException("Los datos de la ciudad son inválidos.");

            // 1. Verificar si ya existe este destino en nuestra base de datos (por nombre y latitud/longitud aproximada)
            // Esto evita crear duplicados cuando múltiples usuarios califican la misma ciudad.
            var minLat = (float)city.Latitude - 0.01f;
            var maxLat = (float)city.Latitude + 0.01f;
            
            var existingDestino = await _destinoRepository.FirstOrDefaultAsync(d => 
                d.Nombre == city.Name && 
                d.Latitud >= minLat && d.Latitud <= maxLat);

            if (existingDestino != null)
            {
                return ObjectMapper.Map<DestinoTuristico, DestinoTuristicoDTO>(existingDestino);
            }

            var pais = await _paisRepository.FirstOrDefaultAsync(p => p.Nombre == city.Country)
                ?? await _paisRepository.InsertAsync(new Pais { Nombre = city.Country ?? "Desconocido" }, autoSave: true);

            var region = await _regionRepository.FirstOrDefaultAsync(r =>
                r.Nombre == city.Region && r.PaisId == pais.Id)
                ?? await _regionRepository.InsertAsync(new Region
                {
                    Nombre = city.Region ?? "Desconocida",
                    Descripcion = $"Región importada desde GeoDB ({city.Region})",
                    PaisId = pais.Id
                }, autoSave: true);

            var destino = new DestinoTuristico
            {
                Nombre = city.Name,
                Poblacion = city.Population ?? 0,
                Latitud = (float)city.Latitude,
                Longuitud = (float)city.Longitude,
                ImagenUrl = "https://via.placeholder.com/300x200.png?text=Destino",
                CalificacionGeneral = 0,
                RegionId = region.Id
            };

            await _destinoRepository.InsertAsync(destino, autoSave: true);

            return ObjectMapper.Map<DestinoTuristico, DestinoTuristicoDTO>(destino);
        }
    }

}



