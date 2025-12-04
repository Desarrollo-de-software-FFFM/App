using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ExploraYa1.Notificaciones
{
    public interface INotificacionAppService : IApplicationService
    {
        Task<List<NotificacionDTO>> GetMisNotificacionesAsync();

        Task MarcarLeidaAsync(Guid id);
        Task MarcarNoLeidaAsync(Guid id);
        Task CrearNotificacionCambioDestinoAsync(Guid destinoId, string detalle);
    }

}
