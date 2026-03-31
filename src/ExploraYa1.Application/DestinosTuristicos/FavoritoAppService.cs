using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExploraYa1.Destinos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace ExploraYa1.DestinosTuristicos
{
    [Authorize]
    public class FavoritoAppService : ApplicationService, IFavoritoAppService
    {
        private readonly IRepository<Favorito, Guid> _favoritoRepository;
        private readonly IRepository<DestinoTuristico, Guid> _destinoRepository;
        private readonly ICurrentUser _currentUser;

        public FavoritoAppService(
            IRepository<Favorito, Guid> favoritoRepository,
            IRepository<DestinoTuristico, Guid> destinoRepository,
            ICurrentUser currentUser)
        {
            _favoritoRepository = favoritoRepository;
            _destinoRepository = destinoRepository;
            _currentUser = currentUser;
        }

        public virtual async Task<FavoritoDto> AgregarFavoritoAsync(Guid destinoTuristicoId)
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debes iniciar sesión para agregar favoritos.");

            var userId = _currentUser.GetId();

            var destino = await _destinoRepository.FindAsync(d => d.Id == destinoTuristicoId);
            if (destino == null)
                throw new UserFriendlyException("Destino no encontrado.");

            var existente = await _favoritoRepository.FindAsync(f =>
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
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debes iniciar sesión para eliminar favoritos.");

            var userId = _currentUser.GetId();

            var favorito = await _favoritoRepository.FindAsync(f =>
                f.DestinoTuristicoId == destinoTuristicoId && f.UserId == userId);

            if (favorito == null)
                throw new EntityNotFoundException(typeof(Favorito), destinoTuristicoId);

            await _favoritoRepository.DeleteAsync(favorito, autoSave: true);
        }

        public virtual async Task<List<FavoritoDto>> ObtenerMisFavoritosAsync()
        {
            if (!_currentUser.IsAuthenticated)
                throw new AbpAuthorizationException("Debe estar autenticado para ver sus favoritos.");

            var userId = _currentUser.GetId();

            var favoritos = await _favoritoRepository.GetListAsync(f => f.UserId == userId);

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
