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

        public async Task<List<CalificacionDto>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debe estar autenticado para ver perfiles.");

            var opiniones = await _opinionRepository.GetListAsync(o => o.UserId == usuarioId);

            // Intentar cargar nombres de destinos
            var destinosIds = opiniones.Select(o => o.DestinoTuristicoId).Distinct().ToList();
            var destinos = new Dictionary<Guid, string>();
            
            try
            {
                var destinoRepository = LazyServiceProvider.LazyGetRequiredService<IRepository<DestinoTuristico, Guid>>();
                var destinosEntities = await destinoRepository.GetListAsync(d => destinosIds.Contains(d.Id));
                foreach(var d in destinosEntities)
                {
                    destinos[d.Id] = d.Nombre;
                }
            }
            catch
            {
                // Ignorar si falla
            }

            return opiniones.Select(o => new CalificacionDto
            {
                Id = o.Id,
                UserId = o.UserId,
                DestinoTuristicoId = o.DestinoTuristicoId,
                DestinoNombre = destinos.ContainsKey(o.DestinoTuristicoId) ? destinos[o.DestinoTuristicoId] : "Destino",
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
            
            // To get UserNames, we fetch users for these IDs
            var userIds = opiniones.Select(o => o.UserId).Distinct().ToList();
            var users = new Dictionary<Guid, string>();
            
            try
            {
                var userRepository = LazyServiceProvider.LazyGetRequiredService<IRepository<Volo.Abp.Identity.IdentityUser, Guid>>();
                var identityUsers = await userRepository.GetListAsync(u => userIds.Contains(u.Id));
                foreach(var u in identityUsers)
                {
                    users[u.Id] = u.UserName;
                }
            }
            catch
            {
                // Ignore if IdentityUser repository is not accessible directly
            }

            return opiniones.Select(o => new CalificacionDto
            {
                Id = o.Id,
                UserId = o.UserId,
                UserName = users.ContainsKey(o.UserId) ? users[o.UserId] : "Usuario",
                DestinoTuristicoId = o.DestinoTuristicoId,
                Comentario = o.Comentario,
                Puntuacion = o.Puntuacion,
                CreationTime = o.CreationTime
            }).ToList();
        }
    }
}
