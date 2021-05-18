using System;
using System.Collections.Generic;

namespace Models
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<QuestionOption> Options { get; set; }
        public string CorrectAnswer { get; set; }
        public int Marks { get; set; }
        public Guid ExamId { get; set; }
        public Exam Exam { get; set; }
    }
}