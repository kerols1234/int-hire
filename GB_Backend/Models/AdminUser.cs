using GB_Backend.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace GB_Backend.Models
{
    public class AdminUser : IdentityUser
    {
        public string Name { get; set; }
        public string BirthDay { get; set; }
        public Gender Gender { get; set; }
    }
}
