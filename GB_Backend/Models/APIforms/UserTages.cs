using System.Collections.Generic;

namespace GB_Backend.Models.APIforms
{
    public class UserTages
    {
        public UserTages()
        {
            Tags = new HashSet<string>();
        }
        public ICollection<string> Tags { get; set; }
    }
}
