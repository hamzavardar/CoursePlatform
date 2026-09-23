namespace CoursePlatform.Models;

public class LessonProgress
{
    public int Id { get; set; }

    public string StudentId { get; set; } = string.Empty;
    public ApplicationUser Student { get; set; } = null!;

    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public bool IsCompleted { get; set; } = false;
    public DateTime CompletedAt { get; set; } = DateTime.Now;
}