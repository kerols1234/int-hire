using GB_Backend.Models.Enums;
using System.Collections.Generic;

namespace GB_Backend.Models.APIforms
{
    public class UpdateJob
    {
        public UpdateJob()
        {
            Tags = new HashSet<string>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ExpLevel { get; set; }
        public EducationLevel? EducationLevel { get; set; }
        public Career? Career { get; set; }
        public JobType? JobType { get; set; }
        public int Salary { get; set; }
        public string Requirements { get; set; }
        public string Deadline { get; set; }
        public string Posting_Time { get; set; }
        public virtual ICollection<string> Tags { get; set; }
    }
}
