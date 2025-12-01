using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace ExploraYa1.UserProfiles
{
    public class UserProfile : FullAuditedAggregateRoot<Guid>
    {
        public Guid UserId { get; set; }   // Id del usuario de ABP.Identity

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string FotoUrl { get; set; }

        protected UserProfile() { }

        public UserProfile(Guid id, Guid userId, string nombre, string apellido, string telefono, string fotoUrl)
            : base(id)
        {
            UserId = userId;
            Nombre = nombre;
            Apellido = apellido;
            Telefono = telefono;
            FotoUrl = fotoUrl;
        }
    }
}
