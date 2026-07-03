using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Xunit;

namespace ExploraYa1.Tests.DestinosTuristicos
{
    public class FavoritoAppService_Tests
    {
        private readonly Mock<IRepository<Favorito, Guid>> _favoritoRepoMock;
        private readonly Mock<IRepository<DestinoTuristico, Guid>> _destinoRepoMock;
        private readonly Mock<ICurrentUser> _currentUserMock;

        private readonly FavoritoAppService _service;

        public FavoritoAppService_Tests()
        {
            _favoritoRepoMock = new Mock<IRepository<Favorito, Guid>>();
            _destinoRepoMock = new Mock<IRepository<DestinoTuristico, Guid>>();
            _currentUserMock = new Mock<ICurrentUser>();

            _service = new FavoritoAppService(
                _favoritoRepoMock.Object,
                _destinoRepoMock.Object,
                _currentUserMock.Object
            );
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private void SetupAuthenticatedUser(Guid userId)
        {
            _currentUserMock.Setup(u => u.IsAuthenticated).Returns(true);
            _currentUserMock.Setup(u => u.Id).Returns(userId);
        }

        private void SetupDestinoExists(Guid destinoId)
        {
            var destino = (DestinoTuristico)Activator.CreateInstance(typeof(DestinoTuristico), nonPublic: true)!;
            // FindAsync is the real interface method; FirstOrDefaultAsync extension calls it internally
            _destinoRepoMock
                .Setup(r => r.FindAsync(
                    It.IsAny<Expression<Func<DestinoTuristico, bool>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(destino);
        }

        private void SetupNoExistingFavorito()
        {
            _favoritoRepoMock
                .Setup(r => r.FindAsync(
                    It.IsAny<Expression<Func<Favorito, bool>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Favorito)null!);
        }

        private void SetupExistingFavorito(Guid destinoId, Guid userId)
        {
            var favorito = new Favorito(destinoId, userId);
            _favoritoRepoMock
                .Setup(r => r.FindAsync(
                    It.IsAny<Expression<Func<Favorito, bool>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(favorito);
        }

        private void SetupInsertAsync(out Favorito? captured)
        {
            Favorito? capturedLocal = null;
            _favoritoRepoMock
                .Setup(r => r.InsertAsync(
                    It.IsAny<Favorito>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .Callback<Favorito, bool, CancellationToken>((f, _, __) => capturedLocal = f)
                .ReturnsAsync((Favorito f, bool _, CancellationToken __) => f);
            captured = capturedLocal;
        }

        // ── AgregarFavoritoAsync ──────────────────────────────────────────────

        [Fact]
        public async Task AgregarFavorito_Deberia_Crear_Favorito_Cuando_Todo_Es_Valido()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var destinoId = Guid.NewGuid();

            SetupAuthenticatedUser(userId);
            SetupDestinoExists(destinoId);
            SetupNoExistingFavorito();

            Favorito? favoritoInsertado = null;
            _favoritoRepoMock
                .Setup(r => r.InsertAsync(
                    It.IsAny<Favorito>(),
                    It.Is<bool>(b => b),
                    It.IsAny<CancellationToken>()))
                .Callback<Favorito, bool, CancellationToken>((f, _, __) => favoritoInsertado = f)
                .ReturnsAsync((Favorito f, bool _, CancellationToken __) => f);

            // Act
            var result = await _service.AgregarFavoritoAsync(destinoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(destinoId, result.DestinoTuristicoId);
            Assert.Equal(userId, result.UserId);
            Assert.NotEqual(Guid.Empty, result.Id);

            Assert.NotNull(favoritoInsertado);
            Assert.Equal(destinoId, favoritoInsertado!.DestinoTuristicoId);
            Assert.Equal(userId, favoritoInsertado.UserId);

            _favoritoRepoMock.Verify(r => r.InsertAsync(
                    It.IsAny<Favorito>(),
                    It.Is<bool>(b => b),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task AgregarFavorito_Deberia_LanzarExcepcion_Si_YaEsFavorito()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var destinoId = Guid.NewGuid();

            SetupAuthenticatedUser(userId);
            SetupDestinoExists(destinoId);
            SetupExistingFavorito(destinoId, userId);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<UserFriendlyException>(
                () => _service.AgregarFavoritoAsync(destinoId));

            Assert.Contains("ya está en tus favoritos", ex.Message);
            _favoritoRepoMock.Verify(r => r.InsertAsync(
                    It.IsAny<Favorito>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task AgregarFavorito_Deberia_LanzarExcepcion_Si_UsuarioNoAutenticado()
        {
            // Arrange
            _currentUserMock.Setup(u => u.IsAuthenticated).Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<AbpAuthorizationException>(
                () => _service.AgregarFavoritoAsync(Guid.NewGuid()));
        }

        // ── EliminarFavoritoAsync ─────────────────────────────────────────────

        [Fact]
        public async Task EliminarFavorito_Deberia_Eliminar_Cuando_FavoritoExiste()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var destinoId = Guid.NewGuid();

            SetupAuthenticatedUser(userId);
            SetupExistingFavorito(destinoId, userId);

            _favoritoRepoMock
                .Setup(r => r.DeleteAsync(It.IsAny<Favorito>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.EliminarFavoritoAsync(destinoId);

            // Assert
            _favoritoRepoMock.Verify(r => r.DeleteAsync(
                    It.IsAny<Favorito>(), It.Is<bool>(b => b), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task EliminarFavorito_Deberia_LanzarEntityNotFound_Si_NoExiste()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetupAuthenticatedUser(userId);
            SetupNoExistingFavorito();

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _service.EliminarFavoritoAsync(Guid.NewGuid()));

            _favoritoRepoMock.Verify(r => r.DeleteAsync(
                    It.IsAny<Favorito>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task EliminarFavorito_Deberia_LanzarExcepcion_Si_UsuarioNoAutenticado()
        {
            // Arrange
            _currentUserMock.Setup(u => u.IsAuthenticated).Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<AbpAuthorizationException>(
                () => _service.EliminarFavoritoAsync(Guid.NewGuid()));
        }

        // ── ObtenerMisFavoritosAsync ──────────────────────────────────────────

        [Fact]
        public async Task ObtenerMisFavoritos_Deberia_RetornarLista_CuandoTieneFavoritos()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetupAuthenticatedUser(userId);

            var favoritos = new List<Favorito>
            {
                new Favorito(Guid.NewGuid(), userId),
                new Favorito(Guid.NewGuid(), userId),
            };

            _favoritoRepoMock
                .Setup(r => r.GetListAsync(
                    It.IsAny<Expression<Func<Favorito, bool>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(favoritos);

            // Act
            var result = await _service.ObtenerMisFavoritosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, dto => Assert.Equal(userId, dto.UserId));
        }

        [Fact]
        public async Task ObtenerMisFavoritos_Deberia_RetornarListaVacia_CuandoNoTieneFavoritos()
        {
            // Arrange
            var userId = Guid.NewGuid();
            SetupAuthenticatedUser(userId);

            _favoritoRepoMock
                .Setup(r => r.GetListAsync(
                    It.IsAny<Expression<Func<Favorito, bool>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Favorito>());

            // Act
            var result = await _service.ObtenerMisFavoritosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ObtenerMisFavoritos_Deberia_LanzarExcepcion_Si_UsuarioNoAutenticado()
        {
            // Arrange
            _currentUserMock.Setup(u => u.IsAuthenticated).Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<AbpAuthorizationException>(
                () => _service.ObtenerMisFavoritosAsync());
        }
    }
}
