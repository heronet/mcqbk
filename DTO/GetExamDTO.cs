using System;
using System.Collections.Generic;
using Models;

namespace DTO
{
    public class GetExamDTO
    {
        public string Title { get; set; }
        public Guid Id { get; set; }
        public int Attendees { get; set; } = 0;
        public string Subject { get; set; }
        public string CreatorId { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Duration { get; set; }
        public double TotalMarks { get; set; }
        public double MarksObtained { get; set; }
        public double NegativeMarks { get; set; }
        public bool SubmissionEnabled { get; set; }
        public bool Participated { get; set; }
        public bool NewSubmission { get; set; }
        public List<QuestionDTO> Questions { get; set; }
    }
}