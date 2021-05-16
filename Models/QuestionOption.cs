using System;

namespace Models
{
    public class QuestionOption
    {
        public Guid Id { get; set; }
        public string Option { get; set; }
        public Guid QuestionId { get; set; }
        public Question Question { get; set; }
    }
}