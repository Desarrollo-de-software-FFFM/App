using ExploraYa1.Destinos;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

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


        public async Task<CalificacionDto> CrearCalificacionAsync(CrearActualizarCalificacionDTO input)
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
        // Editar
        public async Task<CalificacionDto> EditarCalificacionAsync(Guid destinoId, CrearActualizarCalificacionDTO input)
        {
            var userId = _currentUser.Id.Value;

            var calificacion = await _opinionRepository.FirstOrDefaultAsync(
                o => o.DestinoTuristicoId == destinoId && o.UserId == userId);

            if (calificacion == null)
                throw new UserFriendlyException("No tienes calificación para este destino.");

            calificacion.Puntuacion = input.Puntuacion;
            calificacion.Comentario = input.Comentario;

            await _opinionRepository.UpdateAsync(calificacion, true);

            return new CalificacionDto
            {
                DestinoTuristicoId = destinoId,
                UserId = userId,
                Puntuacion = calificacion.Puntuacion,
                Comentario = calificacion.Comentario
            };
        }
        //5.3 Eliminar
        public async Task EliminarCalificacionAsync(Guid destinoId)
        {
            var userId = _currentUser.Id.Value;

            var calificacion = await _opinionRepository.FirstOrDefaultAsync(
                o => o.DestinoTuristicoId == destinoId && o.UserId == userId);

            if (calificacion == null)
                throw new UserFriendlyException("No hay calificación que eliminar.");

            await _opinionRepository.DeleteAsync(calificacion);
        }
        //5.4 Promedio
        public async Task<double> ObtenerPromedioAsync(Guid destinoId)
        {
            var lista = await _opinionRepository.GetListAsync(o => o.DestinoTuristicoId == destinoId);

            return lista.Any() ? lista.Average(o => o.Puntuacion) : 0;
        }
        //5.5 Listar comentarios
        public async Task<List<CalificacionDto>> ListarComentariosAsync(Guid destinoId)
        {
            var opiniones = await _opinionRepository.GetListAsync(
                o => o.DestinoTuristicoId == destinoId && o.Comentario != null && o.Comentario != ""
            );

            return opiniones.Select(o => new CalificacionDto
            {
                DestinoTuristicoId = o.DestinoTuristicoId,
                UserId = o.UserId,
                Comentario = o.Comentario,
                Puntuacion = o.Puntuacion
            }).ToList();
        }
    }

}


