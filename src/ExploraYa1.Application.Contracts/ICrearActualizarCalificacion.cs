using ExploraYa1.Destinos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExploraYa1.Destinos
{
    public interface ICrearActualizarCalificacion
    {
        Task<CalificacionDto> CrearCalificacionAsync(CrearActualizarCalificacionDTO input);
    }
}
