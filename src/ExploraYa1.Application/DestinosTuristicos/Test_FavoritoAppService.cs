using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;
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

        [Fact]
        public async Task AgregarFavorito_Deberia_Crear_Favorito_Cuando_Todo_Es_Valido()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var destinoId = Guid.NewGuid();

            _currentUserMock.Setup(u => u.IsAuthenticated).Returns(true);
            _currentUserMock.Setup(u => u.Id).Returns(userId);

            var destino = CreateDestinoTuristico(destinoId);

            _destinoRepoMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<DestinoTuristico, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(destino);

            _favoritoRepoMock
                .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Favorito, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Favorito)null!);

            Favorito? favoritoInsertado = null;

            _favoritoRepoMock
                .Setup(r => r.InsertAsync(
                    It.IsAny<Favorito>(),
                    It.Is<bool>(b => b),              // autoSave
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

        private DestinoTuristico CreateDestinoTuristico(Guid id)
        {
            var destino = (DestinoTuristico)Activator.CreateInstance(
                typeof(DestinoTuristico),
                nonPublic: true)!;

            return destino;
        }
    }
}
