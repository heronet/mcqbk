using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class ApplicationDbContext : IdentityDbContext<EntityUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<EntityUser>().HasMany(u => u.ParticipatedExams);

            builder.Entity<Exam>().HasOne(e => e.Creator).WithMany(u => u.ParticipatedExams);
        }
        public DbSet<Exam> Exams { get; set; }
    }
}