using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class EntityUser : IdentityUser
    {
        public ICollection<Exam> Exams { get; set; }
        public DateTime? LastActive { get; set; } = DateTime.UtcNow;
    }
}