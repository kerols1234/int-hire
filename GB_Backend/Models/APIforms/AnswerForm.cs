using System.ComponentModel.DataAnnotations;

namespace GB_Backend.Models.APIforms
{
    public class AnswerForm
    {
        public int Id { get; set; }
        [Required]
        public string answer { get; set; }
    }
}
