using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class EntityUser : IdentityUser
    {
        public ICollection<Exam> CreatedExams { get; set; }
        public ICollection<Exam> ParticipatedExams { get; set; }
        public DateTime? LastActive { get; set; } = DateTime.UtcNow;
    }
}