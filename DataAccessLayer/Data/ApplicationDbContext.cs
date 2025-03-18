using DataAccessLayer.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace DataAccessLayer.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<CourseTrainer> CourseTrainers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Define composite primary key for CourseTrainer
            builder.Entity<CourseTrainer>()
                .HasKey(ct => new { ct.CourseId, ct.TrainerId });

            // Define foreign key relationships for CourseTrainer
            builder.Entity<CourseTrainer>()
                .HasOne(ct => ct.Course)
                .WithMany(c => c.CourseTrainers)
                .HasForeignKey(ct => ct.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CourseTrainer>()
                .HasOne(ct => ct.Trainer)
                .WithMany(t => t.CourseTrainers)
                .HasForeignKey(ct => ct.TrainerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Payment>()
         .HasOne(p => p.CourseTrainer)
         .WithMany(ct => ct.Payments)
         .HasForeignKey(p => new { p.CourseId, p.TrainerId })
         .OnDelete(DeleteBehavior.NoAction);
            base.OnModelCreating(builder);
        }
    }
}
