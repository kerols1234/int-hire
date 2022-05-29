using GB_Backend.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace GB_Backend.Models.APIforms
{
    public class RegisterApplicant
    {
        [Required]
        public string Name { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(11, ErrorMessage = "Invalid Mobile Number", MinimumLength = 11)]
        [RegularExpression(@"^01([0-9]{9})", ErrorMessage = "Invalid Mobile Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public String BirthDay { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public MilitaryStatus MilitaryStatus { get; set; }
        public string TwitterUsername { get; set; }
        [Required]
        public EducationLevel EducationLevel { get; set; }
    }
}