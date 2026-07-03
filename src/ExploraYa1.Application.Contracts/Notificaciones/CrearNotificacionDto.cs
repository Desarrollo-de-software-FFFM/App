using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExploraYa1.Notificaciones
{
    public class CrearNotificacionDto
    {
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public Guid? DestinoId { get; set; }
    }

}
