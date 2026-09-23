using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CoursePlatform.ViewModels;

public class EditCourseViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kurs başlığı zorunludur.")]
    [Display(Name = "Kurs Başlığı")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kurs açıklaması zorunludur.")]
    [Display(Name = "Kurs Açıklaması")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
    [Display(Name = "Kategori")]
    public int CategoryId { get; set; }

    public string? CurrentCoverImageUrl { get; set; }

    [Display(Name = "Yeni Kapak Görseli (İsteğe Bağlı)")]
    public IFormFile? NewCoverImage { get; set; }
}