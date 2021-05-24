using System;

namespace DTO
{
    public class ParticipantDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public double MarksObtained { get; set; }
        public string ExamCreatorId { get; set; }
    }
}