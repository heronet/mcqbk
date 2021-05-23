using System;
using System.Collections.Generic;

namespace Models
{
    public class Exam
    {
        public string Title { get; set; }
        public Guid Id { get; set; }
        public int Attendees { get; set; } = 0;
        public Subject Subject { get; set; }
        public string CreatorId { get; set; }
        public EntityUser Creator { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int Duration { get; set; }
        public int TotalMarks { get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<EntityUser> Participients { get; set; }
    }
    public enum Subject
    {
        English,
        Bangla,
        ICT,
        Biology,
        Physics,
        Chemistry,
        Math,
        BanglaShahitto,
        EnglishLiterature,
        Geography,
        CriticalReasoning,
        MathReasoning,
        NoitikotaMullobodh,
        Science,
        Sushasan,
        BangladeshAffairs,
        InternationalAffairs,
        MentalEfficiency,
        ComputerAndIT,
        Multi
    }
}