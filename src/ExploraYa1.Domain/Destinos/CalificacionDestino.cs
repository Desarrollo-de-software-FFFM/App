using System;
using Volo.Abp.Domain.Entities.Auditing;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace ExploraYa1.Destinos
{

    // Constructor para uso de la aplicación (asegura invariantes)
 
    public class CalificacionDestino : AuditedAggregateRoot<Guid>, IUserOwned
    {

        public CalificacionDestino(Guid destinoTuristicoId, Guid userId, int puntuacion, string comentario)
        {
            DestinoTuristicoId = destinoTuristicoId;
            UserId = userId;
            Puntuacion = puntuacion;
            Comentario = comentario;
        }

        // Constructor parameterless para EF Core
        protected CalificacionDestino()
        {
        }
       
        
        public Guid DestinoTuristicoId { get; set; }
        public Guid UserId { get; set; }
        public int Puntuacion { get; set; }
        public string Comentario { get; set; }
   
        
    }



    public interface IUserOwned
    {
        Guid UserId { get; set; }
    }
}