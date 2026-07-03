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

        // ------------------- Crear calificación -------------------
        public async Task<CalificacionDto> CrearCalificacionAsync(CrearActualizarCalificacionDTO input)
        {
            return await _crearOpinionService.CrearCalificacionAsync(input);
        }

        // ------------------- Obtener calificaciones por usuario -------------------
        public async Task<List<CalificacionDto>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debe estar autenticado para ver sus opiniones.");

            if (_currentUser.Id != usuarioId)
                throw new AbpAuthorizationException("No tiene permiso para ver las opiniones de otro usuario.");

            var opiniones = await _opinionRepository.GetListAsync(o => o.UserId == usuarioId);

            return opiniones.Select(o => new CalificacionDto
            {
                Id = o.Id,
                UserId = o.UserId,
                DestinoTuristicoId = o.DestinoTuristicoId,
                Comentario = o.Comentario,
                Puntuacion = o.Puntuacion,
                CreationTime = o.CreationTime
            }).ToList();
        }

        // ------------------- Editar calificación -------------------
        public async Task<CalificacionDto> EditarCalificacionAsync(Guid destinoId, CrearActualizarCalificacionDTO input)
        {
            var userId = _currentUser.Id.Value;

            var calificacion = await _opinionRepository.FirstOrDefaultAsync(
                o => o.DestinoTuristicoId == destinoId && o.UserId == userId);

            if (calificacion == null)
                throw new UserFriendlyException("No tienes calificación para este destino.");

            calificacion.Puntuacion = input.Puntuacion;
            calificacion.Comentario = input.Comentario ?? string.Empty;

            await _opinionRepository.UpdateAsync(calificacion, autoSave: true);

            return new CalificacionDto
            {
                Id = calificacion.Id,
                UserId = calificacion.UserId,
                DestinoTuristicoId = calificacion.DestinoTuristicoId,
                Comentario = calificacion.Comentario,
                Puntuacion = calificacion.Puntuacion,
                CreationTime = calificacion.CreationTime
            };
        }

        // ------------------- Eliminar calificación -------------------
        public async Task EliminarCalificacionAsync(Guid destinoId)
        {
            var userId = _currentUser.Id.Value;

            var calificacion = await _opinionRepository.FirstOrDefaultAsync(
                o => o.DestinoTuristicoId == destinoId && o.UserId == userId);

            if (calificacion == null)
                throw new UserFriendlyException("No hay calificación que eliminar.");

            await _opinionRepository.DeleteAsync(calificacion);
        }

        // ------------------- Obtener promedio de calificaciones -------------------
        public async Task<double> ObtenerPromedioAsync(Guid destinoId)
        {
            var lista = await _opinionRepository.GetListAsync(o => o.DestinoTuristicoId == destinoId);
            return lista.Any() ? lista.Average(o => o.Puntuacion) : 0;
        }

        // ------------------- Listar comentarios de un destino -------------------
        public async Task<List<CalificacionDto>> ListarComentariosAsync(Guid destinoId)
        {
            var opiniones = await _opinionRepository.GetListAsync(
                o => o.DestinoTuristicoId == destinoId && !string.IsNullOrWhiteSpace(o.Comentario)
            );

            return opiniones.Select(o => new CalificacionDto
            {
                Id = o.Id,
                UserId = o.UserId,
                DestinoTuristicoId = o.DestinoTuristicoId,
                Comentario = o.Comentario,
                Puntuacion = o.Puntuacion,
                CreationTime = o.CreationTime
            }).ToList();
        }
    }
}
