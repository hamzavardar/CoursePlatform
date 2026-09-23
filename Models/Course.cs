namespace CoursePlatform.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public decimal Price { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Kategori İlişkisi
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    // Öğretmen (Kullanıcı) İlişkisi
    public string TeacherId { get; set; } = string.Empty;
    public ApplicationUser Teacher { get; set; } = null!;

    // Dersler ve Kayıtlar
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}