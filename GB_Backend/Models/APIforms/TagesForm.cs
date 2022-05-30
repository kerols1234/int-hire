using System.Collections.Generic;

namespace GB_Backend.Models.APIforms
{
    public class TagesForm
    {
        public TagesForm()
        {
            Tags = new HashSet<string>();
        }
        public ICollection<string> Tags { get; set; }
    }
}
