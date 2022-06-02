using System.Collections.Generic;

namespace GB_Backend.Models.APIforms
{
    public class TestForm
    {
        public TestForm()
        {
            Answers = new HashSet<AnswerForm>();
        }
        public HashSet<AnswerForm> Answers { get; set; }
        public int JobId { get; set; }
    }
}
