using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CoursePlatform.ViewModels;

public class EditLessonViewModel
{
    public int Id { get; set; }
    public int CourseId { get; set; }

    [Required(ErrorMessage = "Ders başlığı zorunludur.")]
    [Display(Name = "Ders Başlığı")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Ders Açıklaması")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Sıra numarası zorunludur.")]
    [Display(Name = "Ders Sırası")]
    public int Order { get; set; }

    public string CurrentVideoUrl { get; set; } = string.Empty;

    [Display(Name = "Yeni Video Dosyası (.mp4 - İsteğe Bağlı)")]
    public IFormFile? NewVideoFile { get; set; }
}