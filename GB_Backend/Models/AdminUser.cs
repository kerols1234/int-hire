using GB_Backend.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace GB_Backend.Models
{
    public class AdminUser : IdentityUser
    {
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
        public Gender Gender { get; set; }
    }
}
