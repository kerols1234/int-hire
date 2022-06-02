using GB_Backend.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace GB_Backend.Models
{
    public class ApplicantUser : IdentityUser
    {
        public ApplicantUser()
        {
            Tags = new HashSet<Tag>();
        }
        public string Name { get; set; }
        public string BirthDay { get; set; }
        public Gender Gender { get; set; }
        public MilitaryStatus MilitaryStatus { get; set; }
        public string TwitterUsername { get; set; }
        public EducationLevel EducationLevel { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }

    public enum MilitaryStatus
    {
        Postponed,
        Exempted,
        Completed,
        NotApplicable
    }
}
