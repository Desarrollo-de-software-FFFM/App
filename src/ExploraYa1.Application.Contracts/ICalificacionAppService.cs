using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using ExploraYa1.Destinos;

namespace ExploraYa1.Destinos
{
    public interface ICalificacionAppService : IApplicationService
    {
        Task<CalificacionDto> CrearCalificacionAsync(CrearActualizarCalificacionDTO input);
        Task<List<CalificacionDto>> ObtenerPorUsuarioAsync(Guid usuarioId);

        //NUEVAS FIRMAS
        Task<CalificacionDto> EditarCalificacionAsync(Guid destinoId, CrearActualizarCalificacionDTO input);

        Task EliminarCalificacionAsync(Guid destinoId);

        Task<double> ObtenerPromedioAsync(Guid destinoId);

        Task<List<CalificacionDto>> ListarComentariosAsync(Guid destinoId);
    }
}
