using System.ComponentModel.DataAnnotations;

namespace GB_Backend.Models.APIforms
{
    public class NotificationForm
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ReceiverEmail { get; set; }
        public string Date { get; set; }
    }
}
