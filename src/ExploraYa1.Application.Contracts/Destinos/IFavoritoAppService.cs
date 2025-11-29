using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ExploraYa1.Destinos
{
    public interface IFavoritoAppService : IApplicationService
    {
        Task<FavoritoDto> AgregarFavoritoAsync(Guid destinoTuristicoId);
        Task EliminarFavoritoAsync(Guid destinoTuristicoId);
        Task<List<FavoritoDto>> ObtenerPorUsuarioAsync(Guid usuarioId);
    }
}