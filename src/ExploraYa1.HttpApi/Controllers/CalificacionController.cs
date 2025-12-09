using ExploraYa1.Destinos;
using ExploraYa1.DestinosTuristicos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ExploraYa1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalificacionController : ControllerBase
    {
        private readonly ICalificacionAppService _service;

        public CalificacionController(ICalificacionAppService service)
        {
            _service = service;
        }

        [HttpPost("crear")]
        public async Task<CalificacionDto> Crear([FromBody] CrearActualizarCalificacionDTO input)
        {
            return await _service.CrearCalificacionAsync(input);
        }

        [HttpPut("editar/{destinoId}")]
        public async Task<CalificacionDto> Editar(Guid destinoId, [FromBody] CrearActualizarCalificacionDTO input)
        {
            return await _service.EditarCalificacionAsync(destinoId, input);
        }

        [HttpDelete("eliminar/{destinoId}")]
        public async Task Eliminar(Guid destinoId)
        {
            await _service.EliminarCalificacionAsync(destinoId);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<List<CalificacionDto>> ObtenerPorUsuario(Guid usuarioId)
        {
            return await _service.ObtenerPorUsuarioAsync(usuarioId);
        }

        [HttpGet("promedio/{destinoId}")]
        public async Task<double> Promedio(Guid destinoId)
        {
            return await _service.ObtenerPromedioAsync(destinoId);
        }

        [HttpGet("comentarios/{destinoId}")]
        public async Task<List<CalificacionDto>> Comentarios(Guid destinoId)
        {
            return await _service.ListarComentariosAsync(destinoId);
        }
    }
}
