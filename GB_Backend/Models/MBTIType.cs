using System.ComponentModel.DataAnnotations;

namespace GB_Backend.Models
{
    public class MBTIType
    {
        [Key]
        public string Type { get; set; }
        public string Introduction { get; set; }
        public string StrengthsAndWeakness { get; set; }
        public string RomanticRelationship { get; set; }
        public string Friendships { get; set; }
        public string Parenthood { get; set; }
        public string CareerPaths { get; set; }
        public string WorkplaceHabits { get; set; }
        public string Conclusion { get; set; }
        public string Celebrities { get; set; }
        public string Description { get; set; }
        public string Nickname { get; set; }
        public string Definition { get; set; }
    }
}
