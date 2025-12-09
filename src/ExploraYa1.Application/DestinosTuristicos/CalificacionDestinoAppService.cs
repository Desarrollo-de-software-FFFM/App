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
        private readonly IRepository<CalificacionDestino, Guid> _opinionRepository;
        private readonly ICurrentUser _currentUser;

        public CalificacionAppService(
            IRepository<CalificacionDestino, Guid> opinionRepository,
            ICurrentUser currentUser)
        {
            _opinionRepository = opinionRepository;
            _currentUser = currentUser;
        }

        // ------------------- 5.1 Crear calificación -------------------
        public async Task<CalificacionDto> CrearCalificacionAsync(CrearActualizarCalificacionDTO input)
        {
            var calificacion = new CalificacionDestino(
                input.DestinoTuristicoId,
                _currentUser.Id.Value,
                input.Puntuacion,
                input.Comentario ?? string.Empty
            );

            await _opinionRepository.InsertAsync(calificacion, autoSave: true);

            return ObjectMapper.Map<CalificacionDestino, CalificacionDto>(calificacion);
        }

        // ------------------- Obtener calificaciones por usuario -------------------
        public async Task<List<CalificacionDto>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debe estar autenticado para ver sus opiniones.");

            if (_currentUser.Id != usuarioId)
                throw new AbpAuthorizationException("No tiene permiso para ver las opiniones de otro usuario.");

            var opiniones = await _opinionRepository.GetListAsync(o => o.UserId == usuarioId);

            return ObjectMapper.Map<List<CalificacionDestino>, List<CalificacionDto>>(opiniones);
        }

        // ------------------- 5.3 Editar calificación -------------------
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

            return ObjectMapper.Map<CalificacionDestino, CalificacionDto>(calificacion);
        }

        // ------------------- 5.3 Eliminar calificación -------------------
        public async Task EliminarCalificacionAsync(Guid destinoId)
        {
            var userId = _currentUser.Id.Value;

            var calificacion = await _opinionRepository.FirstOrDefaultAsync(
                o => o.DestinoTuristicoId == destinoId && o.UserId == userId);

            if (calificacion == null)
                throw new UserFriendlyException("No hay calificación que eliminar.");

            await _opinionRepository.DeleteAsync(calificacion);
        }

        // ------------------- 5.4 Obtener promedio -------------------
        public async Task<double> ObtenerPromedioAsync(Guid destinoId)
        {
            var lista = await _opinionRepository.GetListAsync(o => o.DestinoTuristicoId == destinoId);
            return lista.Any() ? lista.Average(o => o.Puntuacion) : 0;
        }

        // ------------------- 5.5 Listar comentarios -------------------
        public async Task<List<CalificacionDto>> ListarComentariosAsync(Guid destinoId)
        {
            var opiniones = await _opinionRepository.GetListAsync(
                o => o.DestinoTuristicoId == destinoId && !string.IsNullOrWhiteSpace(o.Comentario)
            );

            return ObjectMapper.Map<List<CalificacionDestino>, List<CalificacionDto>>(opiniones);
        }
    }
}
