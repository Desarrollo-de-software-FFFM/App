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

        public async Task<DestinoTuristicoDTO> CrearDesdeGeoDbAsync(int cityId)
        {
            var city = await _citySearchService.GetCityDetailsAsync(cityId);

            if (city == null)
                throw new UserFriendlyException("No se encontró la ciudad en GeoDB.");

            var pais = await _paisRepository.FirstOrDefaultAsync(p => p.Nombre == city.Country)
                ?? await _paisRepository.InsertAsync(new Pais { Nombre = city.Country }, autoSave: true);

            var region = await _regionRepository.FirstOrDefaultAsync(r =>
                r.Nombre == city.Region && r.PaisId == pais.Id)
                ?? await _regionRepository.InsertAsync(new Region
                {
                    Nombre = city.Region,
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

            await _destinoRepository.InsertAsync(destino);

            return ObjectMapper.Map<DestinoTuristico, DestinoTuristicoDTO>(destino);
        }
    }

}



