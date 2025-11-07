using ExploraYa1.Destinos;
using NSubstitute;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Volo.Abp.Users;
using Xunit;
using ExploraYa1;

namespace ExploraYa1.CalificacionesTest
{
    [Collection(ExploraYa1TestConsts.CollectionDefinitionName)]
    public abstract class OpinionTestAppServiceTest<TStartupModule>
        : ExploraYa1ApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly ICalificacionAppService _opinionService;

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
            ((int)result.Puntuacion).ShouldBeInRange(1,5);
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
            // Requisito 1: Requerir Autenticación (se verifica al inicio)
            CurrentUser.IsAuthenticated.ShouldBeTrue();

            var destinoId = Guid.NewGuid();

            var input = new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = destinoId,
                Puntuacion = 3,
                Comentario = "Correcto."
            };

            var opinion = await _opinionService.CrearCalificacionAsync(input);

         
            

            // 🔸 Simular un contexto sin autenticación
            var currentUserMock = GetRequiredService<ICurrentUser>();
            currentUserMock.IsAuthenticated.Returns(false);
            currentUserMock.Id.Returns((Guid?)null);

            // Verificar que al intentar la operación sin autenticación, se lance la excepción de autorización
            await Should.ThrowAsync<AbpAuthorizationException>(
                async () => await _opinionService.ObtenerPorUsuarioAsync(Guid.NewGuid())
            );
        }


        //Asegurar que el endpoint de crear una opinion falla con 401 si no se provee token

        [Fact]
        public async Task CrearOpinionAsync_DebeFallarCon401SiNoSeProveeToken()
        {
            // Simular un contexto sin autenticación
            CurrentUser.IsAuthenticated.Returns(false);
            CurrentUser.Id.Returns((Guid?)null);
            var input = new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = Guid.NewGuid(),
                Puntuacion = 2,
                Comentario = "No me gustó mucho."
            };

            // Verificar que al intentar crear una opinión sin autenticación, se lance la excepción de autorización de ABP.
            await Should.ThrowAsync<AbpAuthorizationException>(
                async () => await _opinionService.CrearCalificacionAsync(input)
            );



        }

    }
}