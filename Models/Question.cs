using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<QuestionOption> Options { get; set; }
        public string CorrectAnswer { get; set; }
        public Guid ExamId { get; set; }
        public Exam Exam { get; set; }
    }
}