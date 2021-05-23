using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models;

namespace DTO
{
    public class CreateExamDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        public double NegativeMarks { get; set; }
        [Required]
        public List<QuestionDTO> Questions { get; set; }
    }
}