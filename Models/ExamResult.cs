using System;

namespace Models
{
    public class ExamResult
    {
        public Guid Id { get; set; }
        public Exam Exam { get; set; }
        public Guid ExamId { get; set; }
        public int Score { get; set; }
    }
}