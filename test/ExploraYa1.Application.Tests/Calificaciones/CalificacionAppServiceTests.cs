using ExploraYa1;
using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Testing;
using Volo.Abp.Users;
using Xunit;

namespace ExploraYa1.CalificacionesTest
{
    [Collection(ExploraYa1TestConsts.CollectionDefinitionName)]
    public abstract class OpinionTestAppServiceTest<TStartupModule>
        : ExploraYa1ApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private ICalificacionAppService _opinionService;

        protected OpinionTestAppServiceTest()
        {
            _opinionService = GetRequiredService<ICalificacionAppService>();
        }

        [Fact]
        public async Task CrearOpinionAsync_DebeRetornarOpinionDto()
        {
            var input = new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = Guid.NewGuid(),
                Puntuacion = 5,
                Comentario = "Excelente destino turístico!"
            };

            var result = await _opinionService.CrearCalificacionAsync(input);

            result.ShouldNotBeNull();
            result.DestinoTuristicoId.ShouldBe(input.DestinoTuristicoId);
            result.Puntuacion.ShouldBe(input.Puntuacion);
            ((int)result.Puntuacion).ShouldBeInRange(1, 5);
            result.Comentario.ShouldBe(input.Comentario);
        }

        [Fact]
        public async Task CrearOpinionAsync_NoDebePermitirDuplicados()
        {
            var destinoId = Guid.NewGuid();
            var input = new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = destinoId,
                Puntuacion = 4,
                Comentario = "Muy lindo lugar"
            };

            var primeraOpinion = await _opinionService.CrearCalificacionAsync(input);

            var ex = await Assert.ThrowsAsync<UserFriendlyException>(() => _opinionService.CrearCalificacionAsync(input));
            ex.Message.ShouldBe("Ya has calificado este destino.");
        }

        [Fact]
        public async Task Debe_RespetarFiltroPorUsuario_Y_RequerirAutenticacion()
        {
            // 1️⃣ Login antes de usar el service
            LoginAs(Guid.NewGuid());

            // 2️⃣ Crear una calificación como usuario autenticado
            var destinoId = Guid.NewGuid();
            var input = new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = destinoId,
                Puntuacion = 3,
                Comentario = "Correcto."
            };

            await _opinionService.CrearCalificacionAsync(input);

            // 3️⃣ Logout → usuario no autenticado
            Logout();

            // 4️⃣ Intentar acceder a recurso protegido → debe lanzar 401
            await Should.ThrowAsync<AbpAuthorizationException>(async () =>
                await _opinionService.ObtenerPorUsuarioAsync(Guid.NewGuid())
            );
        }

        [Fact]
        public async Task CrearOpinionAsync_DebeFallarCon401SiNoSeProveeToken()
        {
            CurrentUserMock.IsAuthenticated.Returns(false);
            CurrentUserMock.Id.Returns((Guid?)null);

            var input = new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = Guid.NewGuid(),
                Puntuacion = 2,
                Comentario = "No me gustó mucho."
            };

            await Should.ThrowAsync<AbpAuthorizationException>(
                () => _opinionService.CrearCalificacionAsync(input)
            );
        }



        [Fact]
        public async Task EditarCalificacionAsync_DebeEditarCalificacionPropia()
        {
            var destinoId = Guid.NewGuid();
            var input = new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = destinoId,
                Puntuacion = 3,
                Comentario = "Correcto."
            };
            var opinion = await _opinionService.CrearCalificacionAsync(input);

            var inputEditar = new CrearActualizarCalificacionDTO
            {
                Puntuacion = 5,
                Comentario = "Excelente."
            };
            var resultadoEditar = await _opinionService.EditarCalificacionAsync(destinoId, inputEditar);

            resultadoEditar.Puntuacion.ShouldBe(5);
            resultadoEditar.Comentario.ShouldBe("Excelente.");
            resultadoEditar.UserId.ShouldBe(CurrentUser.Id.Value);
        }

        [Fact]
        public async Task EliminarCalificacionAsync_DebeEliminarCalificacionPropia()
        {
            var destinoId = Guid.NewGuid();
            var input = new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = destinoId,
                Puntuacion = 4,
                Comentario = "Bien."
            };
            await _opinionService.CrearCalificacionAsync(input);

            await _opinionService.EliminarCalificacionAsync(destinoId);

            var opiniones = await _opinionService.ObtenerPorUsuarioAsync(CurrentUser.Id.Value);
            opiniones.ShouldNotContain(op => op.DestinoTuristicoId == destinoId);
        }

        [Fact]
        public async Task ObtenerPromedioAsync_DebeRetornarPromedioCorrecto()
        {
            var destinoId = Guid.NewGuid();

            var usuarios = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var puntuaciones = new[] { 4, 5, 3 };

            // ⚡ Crear un scope de DI para mantener el DbContext vivo durante todo el test
            using (var scope = ServiceProvider.CreateScope())
            {
                var calificacionService = scope.ServiceProvider.GetRequiredService<CalificacionAppService>();

                for (int i = 0; i < usuarios.Length; i++)
                {
                    // Cambiar el usuario actual en la misma instancia del servicio
                    LoginAs(usuarios[i]);

                    var input = new CrearActualizarCalificacionDTO
                    {
                        DestinoTuristicoId = destinoId,
                        Puntuacion = puntuaciones[i],
                        Comentario = $"Calificación {i + 1}"
                    };

                    await calificacionService.CrearCalificacionAsync(input);
                }

                // Calcular promedio usando la misma instancia dentro del scope
                var promedio = await calificacionService.ObtenerPromedioAsync(destinoId);

                promedio.ShouldBe(puntuaciones.Average());
            }
        }








        [Fact]
        public async Task ListarComentariosAsync_DebeRetornarSoloComentariosNoVacios()
        {
            var destinoId = Guid.NewGuid();

            // Comentario válido
            await _opinionService.CrearCalificacionAsync(new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = destinoId,
                Puntuacion = 5,
                Comentario = "Excelente"
            });

            // NO SE PUEDEN crear estos porque causarían AbpValidationException:
            // - Comentario = ""
            // - Comentario = null

            var comentarios = await _opinionService.ListarComentariosAsync(destinoId);

            comentarios.Count.ShouldBe(1);
            comentarios[0].Comentario.ShouldBe("Excelente");
        }


    }
}
