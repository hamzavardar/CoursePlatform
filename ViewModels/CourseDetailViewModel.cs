using CoursePlatform.Models;

namespace CoursePlatform.ViewModels;

public class CourseDetailViewModel
{
    public Course Course { get; set; } = null!;
    public bool IsEnrolled { get; set; }
}