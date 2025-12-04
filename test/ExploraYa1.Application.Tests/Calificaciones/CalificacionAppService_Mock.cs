using ExploraYa1.Destinos;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;
using Xunit;

namespace ExploraYa1.CalificacionesTest
{
    public class CalificacionAppService_AdditionalTests
        : OpinionTestAppServiceTest<ExploraYa1ApplicationTestModule>
    {
        private readonly ICalificacionAppService _service;

        public CalificacionAppService_AdditionalTests()
        {
            _service = GetRequiredService<ICalificacionAppService>();
        }

        // ============================================================
        // 5.3 EDITAR CALIFICACIÓN PROPIA
        // ============================================================

        [Fact]
        public async Task EditarCalificacionAsync_DebeModificarSoloCalificacionPropia()
        {
            var destinoId = Guid.NewGuid();

            var crear = await _service.CrearCalificacionAsync(new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = destinoId,
                Puntuacion = 3,
                Comentario = "Está bien."
            });

            var editar = await _service.EditarCalificacionAsync(crear.DestinoTuristicoId, new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = destinoId,
                Puntuacion = 5,
                Comentario = "Mejoró mucho."
            });

            editar.Puntuacion.ShouldBe(5);
            editar.Comentario.ShouldBe("Mejoró mucho.");
        }

        // ============================================================
        // 5.3 ELIMINAR CALIFICACIÓN PROPIA
        // ============================================================

        [Fact]
        public async Task EliminarCalificacionAsync_DebeEliminarSoloPropia()
        {
            var destinoId = Guid.NewGuid();

            var opinion = await _service.CrearCalificacionAsync(new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = destinoId,
                Puntuacion = 4,
                Comentario = "Muy bueno."
            });

            await _service.EliminarCalificacionAsync(opinion.DestinoTuristicoId);

            // Intentar obtener calificaciones del usuario: no debe aparecer
            var lista = await _service.ObtenerPorUsuarioAsync(CurrentUser.Id.Value);

            lista.Any(x => x.DestinoTuristicoId == opinion.DestinoTuristicoId && x.UserId == opinion.UserId).ShouldBeFalse();
        }

        // ============================================================
        // 5.4 – CONSULTAR PROMEDIO
        // ============================================================

        [Fact]
        public async Task ObtenerPromedio_DeberiaRetornarPromedioCorrecto()
        {
            var destinoId = Guid.NewGuid();

            await _service.CrearCalificacionAsync(new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = destinoId,
                Puntuacion = 4,
                Comentario = "Bien."
            });

            await _service.CrearCalificacionAsync(new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = destinoId,
                Puntuacion = 2,
                Comentario = "No tanto."
            });

            var promedio = await _service.ObtenerPromedioAsync(destinoId);

            promedio.ShouldBe(3); // (4 + 2) / 2 = 3
        }

        // ============================================================
        // 5.5 – LISTAR COMENTARIOS DE UN DESTINO
        // ============================================================

        [Fact]
        public async Task ListarComentarios_DeberiaRetornarSoloDelDestino()
        {
            var destinoA = Guid.NewGuid();
            var destinoB = Guid.NewGuid();

            await _service.CrearCalificacionAsync(new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = destinoA,
                Puntuacion = 5,
                Comentario = "Excelente A"
            });

            await _service.CrearCalificacionAsync(new CrearActualizarCalificacionDTO
            {
                DestinoTuristicoId = destinoB,
                Puntuacion = 1,
                Comentario = "Malo B"
            });

            var lista = await _service.ListarComentariosAsync(destinoA);

            lista.Count.ShouldBe(1);
            lista.First().Comentario.ShouldBe("Excelente A");
        }
    }
}

