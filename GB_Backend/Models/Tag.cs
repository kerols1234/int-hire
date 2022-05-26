using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GB_Backend.Models
{
    public class Tag
    {
        public Tag()
        {
            Jobs = new HashSet<Job>();
            ApplicantUsers = new HashSet<ApplicantUser>();
        }

        [Key]
        public string Name { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }

        public virtual ICollection<ApplicantUser> ApplicantUsers { get; set; }
    }
}
