using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
using ExploraYa1.Notificaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ExploraYa1.DestinosTuristicos
{
    public class DestinoTuristicoAppService :
    CrudAppService<
        DestinoTuristico, //The Book entity
        DestinoTuristicoDTO, //Used to show books
        Guid, //Primary key of the book entity
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CrearActualizarDestinoDTO>, //Used to create/update a book
    IDestinoTuristicoAppService //implement the IBookAppService
    {
        

    private readonly ICitySearchService _citySearchService;

    private readonly INotificacionAppService _notificacionAppService;
    public DestinoTuristicoAppService(
                IRepository<DestinoTuristico, Guid> repository,
                ICitySearchService citySearchService,
                INotificacionAppService notificacionAppService)
                : base(repository)
    {
            _citySearchService = citySearchService;
            _notificacionAppService = notificacionAppService;
    }
    public DestinoTuristicoAppService(IRepository<DestinoTuristico, Guid> repository, ICitySearchService citySearchService)
            : base(repository)
    {
            _citySearchService = citySearchService;
    }
    public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
    {
            return await _citySearchService.SearchCitiesAsync(request);
    }

        public async Task MarcarNotificacionLeidaAsync(Guid notificacionId)
        {
            await _notificacionAppService.MarcarLeidaAsync(notificacionId);
        }

        // 🔹 Notificar cuando un destino es actualizado
        public override async Task<DestinoTuristicoDTO> UpdateAsync(Guid id, CrearActualizarDestinoDTO input)
        {
            var result = await base.UpdateAsync(id, input);

            await _notificacionAppService.CrearNotificacionCambioDestinoAsync(
                id,
                $"El destino '{input.Nombre}' fue actualizado."
            );

            return result;
        }


    }
}




