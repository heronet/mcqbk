using System;
using System.Collections.Generic;

namespace DTO
{
    public class QuestionDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }

    }
}