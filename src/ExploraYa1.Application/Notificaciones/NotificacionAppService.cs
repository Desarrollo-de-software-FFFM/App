using AutoMapper.Internal.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace ExploraYa1.Notificaciones
{
    public class NotificacionAppService : ApplicationService, INotificacionAppService
    {
        private readonly IRepository<Notificacion, Guid> _notiRepo;
        private readonly ICurrentUser _currentUser;

        public NotificacionAppService(
            IRepository<Notificacion, Guid> notiRepo,
            ICurrentUser currentUser)
        {
            _notiRepo = notiRepo;
            _currentUser = currentUser;
        }


        public async Task<List<NotificacionDTO>> GetMisNotificacionesAsync()
        {
            var userId = _currentUser.Id ?? throw new UserFriendlyException("Usuario no identificado");

            var notis = await _notiRepo.GetListAsync(x => x.UserId == userId);

            // Mapeo manual sin AutoMapper
            return notis.Select(n => new NotificacionDTO
            {
                Id = n.Id,
                UserId = n.UserId,
                DestinoId = n.DestinoId,
                Titulo = n.Titulo,
                Mensaje = n.Mensaje,
                Leida = n.Leida
            }).ToList();
        }


        public async Task MarcarLeidaAsync(Guid id)
        {
            var noti = await _notiRepo.GetAsync(id);
            noti.Leida = true;
            await _notiRepo.UpdateAsync(noti);
        }

        public async Task MarcarNoLeidaAsync(Guid id)
        {
            var noti = await _notiRepo.GetAsync(id);
            noti.Leida = false;
            await _notiRepo.UpdateAsync(noti);
        }

        public async Task CrearNotificacionCambioDestinoAsync(Guid destinoId, string detalle)
        {
            var userId = _currentUser.Id ?? throw new UserFriendlyException("Usuario no identificado");

            var noti = new Notificacion(Guid.NewGuid())
            {
                UserId = userId,
                DestinoId = destinoId,
                Titulo = "Actualización en destino turístico",
                Mensaje = detalle,
                Leida = false
            };

            await _notiRepo.InsertAsync(noti);
        }
    }

}
