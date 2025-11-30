using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace ExploraYa1.Notificaciones
{
    public class Notificacion : Entity<Guid>
    {

        public Notificacion(Guid id)
        {
            Id = id; // aquí sí podés asignarlo
        }
        
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Mensaje { get; set; }

        public bool Leida { get; set; } = false;

        public Guid? DestinoId { get; set; } // opcional si la notificación está asociada a un destino

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }

}
