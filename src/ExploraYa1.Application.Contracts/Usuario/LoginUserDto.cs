using System.ComponentModel.DataAnnotations;

namespace ExploraYa1.Usuarios
{
    public class LoginUserDto
    {
        [Required]
        public string UserNameOrEmail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
