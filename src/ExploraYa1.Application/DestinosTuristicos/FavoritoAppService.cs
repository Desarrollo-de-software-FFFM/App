using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExploraYa1.Destinos;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Volo.Abp.Authorization;      
using Microsoft.AspNetCore.Authorization;

namespace ExploraYa1.DestinosTuristicos
{
    [Authorize] 
    public class FavoritoAppService : ApplicationService, IFavoritoAppService
    {
        private readonly IRepository<Favorito, Guid> _favoritoRepository;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;

        public FavoritoAppService(
            IRepository<Favorito, Guid> favoritoRepository,
            IRepository<DestinoTuristico, Guid> destinoRepository)
        {
            _favoritoRepository = favoritoRepository;
            _destinoRepository = destinoRepository;
        }

        public virtual async Task<FavoritoDto> AgregarFavoritoAsync(Guid destinoTuristicoId)
        {
            if (!CurrentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debes iniciar sesión para agregar favoritos.");

            var userId = CurrentUser.GetId(); // extensión de ABP

            var destino = await _destinoRepository.FirstOrDefaultAsync(d => d.Id == destinoTuristicoId);
            if (destino == null)
                throw new UserFriendlyException("Destino no encontrado.");

            var existente = await _favoritoRepository.FirstOrDefaultAsync(f =>
                f.DestinoTuristicoId == destinoTuristicoId && f.UserId == userId);

            if (existente != null)
                throw new UserFriendlyException("El destino ya está en tus favoritos.");

            var favorito = new Favorito(destinoTuristicoId, userId);
            await _favoritoRepository.InsertAsync(favorito, autoSave: true);

            return new FavoritoDto
            {
                Id = favorito.Id,
                DestinoTuristicoId = favorito.DestinoTuristicoId,
                UserId = favorito.UserId,
                CreationTime = favorito.CreationTime
            };
        }

        public virtual async Task EliminarFavoritoAsync(Guid destinoTuristicoId)
        {
            if (!CurrentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debes iniciar sesión para eliminar favoritos.");

            var userId = CurrentUser.GetId();

            var favorito = await _favoritoRepository.FirstOrDefaultAsync(f =>
                f.DestinoTuristicoId == destinoTuristicoId && f.UserId == userId);

            if (favorito == null)
                throw new UserFriendlyException("Favorito no encontrado.");

            await _favoritoRepository.DeleteAsync(favorito, autoSave: true);
        }

        public virtual async Task<List<FavoritoDto>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            if (!CurrentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debe estar autenticado para ver sus favoritos.");

            var currentUserId = CurrentUser.GetId();

            if (currentUserId != usuarioId)
                throw new AbpAuthorizationException("No tiene permiso para ver los favoritos de otro usuario.");

            var favoritos = await _favoritoRepository.GetListAsync(f => f.UserId == usuarioId);

            return favoritos.Select(f => new FavoritoDto
            {
                Id = f.Id,
                DestinoTuristicoId = f.DestinoTuristicoId,
                UserId = f.UserId,
                CreationTime = f.CreationTime
            }).ToList();
        }
    }
}