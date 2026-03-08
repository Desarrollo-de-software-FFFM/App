using System;
using System.ComponentModel.DataAnnotations;

namespace ExploraYa1.Usuarios
{
    public class RegisterUserDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
