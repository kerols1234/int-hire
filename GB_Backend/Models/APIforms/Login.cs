using System.ComponentModel.DataAnnotations;

namespace GB_Backend.Models.APIforms
{
    public class Login
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
