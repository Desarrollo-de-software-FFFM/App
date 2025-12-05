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

        // Constructor PROTEGIDO para Entity Framework Core/Repositorios
        protected UserProfile() { }

        // Constructor de INICIALIZACIÓN (Usado para crear el perfil)
        // Solo requiere el Id (PK de la entidad) y el UserId (FK a AppUsers)
        public UserProfile(Guid id, Guid userId)
            : base(id)
        {
            UserId = userId;
            // Inicializamos las propiedades para garantizar que no sean NULL en la base de datos
            Nombre = string.Empty;
            Apellido = string.Empty;
            Telefono = string.Empty;
            FotoUrl = string.Empty;
        }

    }
}
