using GB_Backend.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace GB_Backend.Models.APIforms
{
    public class UpdateRecruiter
    {
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(11, ErrorMessage = "Invalid Mobile Number", MinimumLength = 11)]
        [RegularExpression(@"^01([0-9]{9})", ErrorMessage = "Invalid Mobile Number")]
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public String BirthDay { get; set; }
        public Gender? Gender { get; set; }
        public string Position { get; set; }
    }
}