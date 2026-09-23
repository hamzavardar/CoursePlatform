using CoursePlatform.Models;

namespace CoursePlatform.ViewModels;

public class CourseWatchViewModel
{
    public Course Course { get; set; } = null!;
    public Lesson CurrentLesson { get; set; } = null!;
    public List<Lesson> Lessons { get; set; } = new List<Lesson>();
    
    // İlerleme Takibi Alanları
    public List<int> CompletedLessonIds { get; set; } = new List<int>();
    public int ProgressPercentage { get; set; }
}