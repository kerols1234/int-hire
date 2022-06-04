using System.Collections.Generic;

namespace GB_Backend.Models.APIModels
{
    public class ResponseModel<T>
    {
        public ResponseModel()
        {
            Errors = new HashSet<ErrorModel>();
        }
        public T Data { get; set; }
        public virtual ICollection<ErrorModel> Errors { get; set; }
        public virtual MetaData Meta { get; set; }
    }
}
