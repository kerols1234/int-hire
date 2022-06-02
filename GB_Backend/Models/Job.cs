using GB_Backend.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GB_Backend.Models
{
    public class Job
    {
        public Job()
        {
            Tags = new HashSet<Tag>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ExpLevel { get; set; }
        public EducationLevel EducationLevel { get; set; }
        public Career Career { get; set; }
        public JobType JobType { get; set; }
        public int Salary { get; set; }
        public string Requirements { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime Posting_Time { get; set; }
        public string RecruiterUserId { get; set; }

        [ForeignKey("RecruiterUserId")]
        public virtual RecruiterUser RecruiterUser { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
    }

    public enum Career
    {
        Student,
        Fresh_Junior,
        Senior,
        Manager,
    }

    public enum JobType
    {
        Full_Time,
        Part_Time,
        Work_From_Home,
        Internship,
    }
}
