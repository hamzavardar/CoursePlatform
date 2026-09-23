using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CoursePlatform.ViewModels;

public class AddLessonViewModel
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ders başlığı zorunludur.")]
    [Display(Name = "Ders Başlığı")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Ders Açıklaması")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sıra numarası zorunludur.")]
    [Display(Name = "Ders Sırası")]
    public int Order { get; set; } = 1;

    [Required(ErrorMessage = "Lütfen bir video dosyası seçin.")]
    [Display(Name = "Video Dosyası (.mp4)")]
    public IFormFile VideoFile { get; set; } = null!;
}