using System;

namespace ExploraYa1.Usuarios
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        // Propiedades de la entidad UserProfile
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string FotoUrl { get; set; }
    }
}

