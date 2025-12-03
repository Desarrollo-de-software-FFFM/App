using System.ComponentModel.DataAnnotations;

namespace ExploraYa1.Usuarios
{
    public class UpdateUserProfileDto
    {
        [Required]
        public string UserName { get; set; }


        [EmailAddress]
        public string Email { get; set; }
    }
}

