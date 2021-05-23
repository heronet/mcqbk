using System;
using System.Collections.Generic;

namespace DTO
{
    public class QuestionDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<OptionDTO> Options { get; set; }
        public OptionDTO CorrectAnswer { get; set; }
        public OptionDTO ProvidedAnswer { get; set; }
        public bool HasMath { get; set; }
        public int Marks { get; set; }
    }
}