using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Xunit;
using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
using Volo.Abp.Users;
using Volo.Abp.Authorization;
using Volo.Abp.Security.Claims;
using Volo.Abp.Domain.Repositories;

namespace ExploraYa1.Application.Tests.DestinosTuristicos
{
    public class CalificacionAppService_Mock_Tests
    {
        private readonly Mock<IRepository<CalificacionDestino, Guid>> _mockRepo;
        private readonly Mock<ICrearActualizarCalificacion> _mockService;
        private readonly Mock<ICurrentUser> _mockCurrentUser;
        private readonly CalificacionAppService _calificacionAppService;

        public CalificacionAppService_Mock_Tests()
        {
            _mockRepo = new Mock<IRepository<CalificacionDestino, Guid>>();
            _mockService = new Mock<ICrearActualizarCalificacion>();
            _mockCurrentUser = new Mock<ICurrentUser>();

            _calificacionAppService = new CalificacionAppService(
                _mockService.Object,
                _mockRepo.Object,
                _mockCurrentUser.Object
            );
        }

        [Fact]
        public async Task CrearCalificacionAsync_DebeLlamarServicio_CuandoUsuarioAutenticado()
        {
            var userId = Guid.NewGuid();
            var destinoId = Guid.NewGuid();

            _mockCurrentUser.Setup(x => x.IsAuthenticated).Returns(true);
            _mockCurrentUser.Setup(x => x.GetId()).Returns(userId);

            var input = new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = destinoId,
                Puntuacion = 5,
                Comentario = "Excelente"
            };

            var dtoReturn = new CalificacionDto
            {
                UserId = userId,
                DestinoTuristicoId = destinoId,
                Puntuacion = 5,
                Comentario = "Excelente"
            };

            _mockService.Setup(x => x.CrearCalificacionAsync(input)).ReturnsAsync(dtoReturn);

            var result = await _calificacionAppService.CrearCalificacionAsync(input);

            result.ShouldNotBeNull();
            result.UserId.ShouldBe(userId);
            result.Puntuacion.ShouldBe(5);
            result.Comentario.ShouldBe("Excelente");

            _mockService.Verify(x => x.CrearCalificacionAsync(input), Times.Once);
        }

        [Fact]
        public async Task CrearCalificacionAsync_DebeFallar_SiNoAutenticado()
        {
            _mockCurrentUser.Setup(x => x.IsAuthenticated).Returns(false);

            var input = new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = Guid.NewGuid(),
                Puntuacion = 3
            };

            await Should.ThrowAsync<AbpAuthorizationException>(async () =>
            {
                await _calificacionAppService.CrearCalificacionAsync(input);
            });
        }

        [Fact]
        public async Task EditarCalificacionAsync_DebeEditarCalificacionPropia()
        {
            var userId = Guid.NewGuid();
            var destinoId = Guid.NewGuid();

            _mockCurrentUser.Setup(x => x.IsAuthenticated).Returns(true);
            _mockCurrentUser.Setup(x => x.GetId()).Returns(userId);

            var calificacionExistente = (CalificacionDestino)Activator.CreateInstance(
                typeof(CalificacionDestino),
                nonPublic: true
            );
            calificacionExistente.UserId = userId;
            calificacionExistente.DestinoTuristicoId = destinoId;
            calificacionExistente.Puntuacion = 3;
            calificacionExistente.Comentario = "Regular";

            // Setup correcto usando Expression<Func<T,bool>> y CancellationToken
            _mockRepo.Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<CalificacionDestino, bool>>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(calificacionExistente);

            var inputEditar = new CrearActualizarCalificacionDTO
            {
                Puntuacion = 5,
                Comentario = "Excelente"
            };

            var result = await _calificacionAppService.EditarCalificacionAsync(destinoId, inputEditar);

            result.Puntuacion.ShouldBe(5);
            result.Comentario.ShouldBe("Excelente");
            result.UserId.ShouldBe(userId);
        }

        [Fact]
        public async Task EliminarCalificacionAsync_DebeEliminarCalificacionPropia()
        {
            var userId = Guid.NewGuid();
            var destinoId = Guid.NewGuid();

            _mockCurrentUser.Setup(x => x.IsAuthenticated).Returns(true);
            _mockCurrentUser.Setup(x => x.GetId()).Returns(userId);

            var calificacionExistente = (CalificacionDestino)Activator.CreateInstance(
                typeof(CalificacionDestino),
                nonPublic: true
            );
            calificacionExistente.UserId = userId;
            calificacionExistente.DestinoTuristicoId = destinoId;

            _mockRepo.Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<CalificacionDestino, bool>>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(calificacionExistente);

            _mockRepo.Setup(x => x.DeleteAsync(
                calificacionExistente,
                It.IsAny<CancellationToken>()
            )).Returns(Task.CompletedTask);

            await _calificacionAppService.EliminarCalificacionAsync(destinoId);

            _mockRepo.Verify(x => x.DeleteAsync(calificacionExistente, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ObtenerPromedioAsync_DebeRetornarPromedio()
        {
            var destinoId = Guid.NewGuid();
            var lista = new List<CalificacionDestino>
            {
                (CalificacionDestino)Activator.CreateInstance(typeof(CalificacionDestino), true),
                (CalificacionDestino)Activator.CreateInstance(typeof(CalificacionDestino), true)
            };
            lista[0].Puntuacion = 5;
            lista[1].Puntuacion = 3;

            _mockRepo.Setup(x => x.GetListAsync(
                It.IsAny<Expression<Func<CalificacionDestino, bool>>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(lista);

            var promedio = await _calificacionAppService.ObtenerPromedioAsync(destinoId);

            promedio.ShouldBe(4.0);
        }

        [Fact]
        public async Task ListarComentariosAsync_DebeRetornarSoloConComentario()
        {
            var destinoId = Guid.NewGuid();
            var lista = new List<CalificacionDestino>
            {
                (CalificacionDestino)Activator.CreateInstance(typeof(CalificacionDestino), true),
                (CalificacionDestino)Activator.CreateInstance(typeof(CalificacionDestino), true),
                (CalificacionDestino)Activator.CreateInstance(typeof(CalificacionDestino), true)
            };
            lista[0].Comentario = "Excelente"; lista[0].Puntuacion = 5;
            lista[1].Comentario = ""; lista[1].Puntuacion = 3;
            lista[2].Comentario = null; lista[2].Puntuacion = 4;

            _mockRepo.Setup(x => x.GetListAsync(
                It.IsAny<Expression<Func<CalificacionDestino, bool>>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(lista.Where(x => !string.IsNullOrEmpty(x.Comentario)).ToList());

            var result = await _calificacionAppService.ListarComentariosAsync(destinoId);

            result.Count.ShouldBe(1);
            result.First().Comentario.ShouldBe("Excelente");
        }

        [Fact]
        public async Task ObtenerPorUsuarioAsync_DebeRetornarSoloOpinionesPropias()
        {
            var userId = Guid.NewGuid();
            _mockCurrentUser.Setup(x => x.IsAuthenticated).Returns(true);
            _mockCurrentUser.Setup(x => x.Id).Returns(userId);

            var lista = new List<CalificacionDestino>
            {
                (CalificacionDestino)Activator.CreateInstance(typeof(CalificacionDestino), true),
                (CalificacionDestino)Activator.CreateInstance(typeof(CalificacionDestino), true)
            };
            lista[0].UserId = userId; lista[0].DestinoTuristicoId = Guid.NewGuid(); lista[0].Puntuacion = 5; lista[0].Comentario = "A";
            lista[1].UserId = userId; lista[1].DestinoTuristicoId = Guid.NewGuid(); lista[1].Puntuacion = 4; lista[1].Comentario = "B";

            _mockRepo.Setup(x => x.GetListAsync(
                It.IsAny<Expression<Func<CalificacionDestino, bool>>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(lista);

            var result = await _calificacionAppService.ObtenerPorUsuarioAsync(userId);

            result.Count.ShouldBe(2);
            result[0].Comentario.ShouldBe("A");
            result[1].Comentario.ShouldBe("B");
        }
    }
}
