using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class EntityUser : IdentityUser
    {
        public ICollection<ExamResult> ParticipatedExams { get; set; }
        public ICollection<IdentityRole> Roles { get; set; }
    }
}