using System.ComponentModel.DataAnnotations;

namespace ExploraYa1.Usuarios
{
    public class UpdateUserProfileDto
    {
        [Required]
        [StringLength(64)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(64)]
        public string Apellido { get; set; }

        // Opcional: Si permites cambiar el username y email
        [StringLength(256)]
        public string UserName { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        // Campos extendidos
        [StringLength(16)]
        public string Telefono { get; set; }

        [StringLength(256)]
        public string FotoUrl { get; set; }
    }
}

