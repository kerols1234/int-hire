using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace GB_Backend.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Viewed { get; set; }
        public NotificationType Type { get; set; }

        public string ReceiverId { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual IdentityUser Receiver { get; set; }
        public string SenderId { get; set; }

        [ForeignKey("SenderId")]
        public virtual IdentityUser Sender { get; set; }
    }

    public enum NotificationType
    {
        Complaint,
        Message
    }
}
