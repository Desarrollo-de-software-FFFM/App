using ExploraYa1.Destinos;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Users;
using Volo.Abp.Domain.Repositories;

namespace ExploraYa1.DestinosTuristicos
{
    [Authorize]
    public class CalificacionAppService : ApplicationService, ICalificacionAppService
    {
        private readonly ICrearActualizarCalificacion _crearOpinionService;
        private readonly IRepository<CalificacionDestino, Guid> _opinionRepository;
        private readonly ICurrentUser _currentUser;

        public CalificacionAppService(
            ICrearActualizarCalificacion crearOpinionService,
            IRepository<CalificacionDestino, Guid> opinionRepository,
            ICurrentUser currentUser)
        {
            _crearOpinionService = crearOpinionService;
            _opinionRepository = opinionRepository;
            _currentUser = currentUser;
        }

        public Task<CalificacionDto> CrearCalificacionAsync(CrearActualizarCalificacionDTO input)
        {
            throw new NotImplementedException();
        }

        public async Task<CalificacionDto> CrearOpinionAsync(CrearActualizarCalificacionDTO input)
        {
            return await _crearOpinionService.CrearCalificacionAsync(input);
        }

        public async Task<List<CalificacionDto>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            // 1️⃣ Verificar autenticación
            if (!_currentUser.IsAuthenticated)
            {
                throw new AbpAuthorizationException("Debe estar autenticado para ver sus opiniones.");
            }

            // 2️⃣ Validar que solo consulte su propia información
            if (_currentUser.Id != usuarioId)
            {
                throw new AbpAuthorizationException("No tiene permiso para ver las opiniones de otro usuario.");
            }

            // 3️⃣ Obtener opiniones filtradas por usuario
            var opiniones = await _opinionRepository.GetListAsync(o => o.UserId == usuarioId);

            // 4️⃣ Mapearlas al DTO
            return opiniones.Select(o => new CalificacionDto
            {
               
                UserId = o.UserId,
                DestinoTuristicoId = o.DestinoTuristicoId,
                Comentario = o.Comentario,
                Puntuacion = o.Puntuacion
            }).ToList();
        }
    }
}
