using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace ExploraYa1.Notificaciones
{
    public class NotificacionDTO : AuditedEntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public bool Leida { get; set; }
        public Guid? DestinoId { get; set; }
        public DateTime Fecha { get; set; }
    }

}
