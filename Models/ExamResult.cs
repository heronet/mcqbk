using System;

namespace Models
{
    public class ExamResult
    {
        public Guid Id { get; set; }
        public Exam Exam { get; set; }
        public Guid ExamId { get; set; }
        public double Score { get; set; }
    }
}