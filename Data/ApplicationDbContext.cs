using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CoursePlatform.Models;

namespace CoursePlatform.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<LessonProgress> LessonProgresses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Decimal Hassasiyet Ayarı
        builder.Entity<Course>()
            .Property(c => c.Price)
            .HasColumnType("decimal(18,2)");

        // Teacher - Course ilişkisi
        builder.Entity<Course>()
            .HasOne(c => c.Teacher)
            .WithMany(u => u.CreatedCourses)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Student - Enrollment ilişkisi
        builder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Course - Enrollment ilişkisi
        builder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // LessonProgress İlişkileri
        builder.Entity<LessonProgress>()
            .HasOne(lp => lp.Student)
            .WithMany()
            .HasForeignKey(lp => lp.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<LessonProgress>()
            .HasOne(lp => lp.Lesson)
            .WithMany()
            .HasForeignKey(lp => lp.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}