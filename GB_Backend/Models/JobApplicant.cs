using System.ComponentModel.DataAnnotations.Schema;

namespace GB_Backend.Models
{
    public class JobApplicant
    {
        public string MBTITypeId { get; set; }

        [ForeignKey("MBTITypeId")]
        public virtual MBTIType MBTIType { get; set; }
        public int JobId { get; set; }
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }
        public string ApplicantUserId { get; set; }
        [ForeignKey("ApplicantUserId")]
        public virtual ApplicantUser ApplicantUser { get; set; }
    }
}
