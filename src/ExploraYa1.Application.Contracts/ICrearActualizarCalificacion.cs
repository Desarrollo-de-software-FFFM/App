using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using ExploraYa1.Destinos;

namespace ExploraYa1.Destinos
{
    public interface ICrearActualizarCalificacion : IApplicationService
    {
        Task<CalificacionDto> CrearCalificacionAsync(CrearActualizarCalificacionDTO input);
    }
}
