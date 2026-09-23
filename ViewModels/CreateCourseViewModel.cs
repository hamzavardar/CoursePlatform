using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CoursePlatform.ViewModels;

public class CreateCourseViewModel
{
    [Required(ErrorMessage = "Kurs başlığı zorunludur.")]
    [Display(Name = "Kurs Başlığı")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kurs açıklaması zorunludur.")]
    [Display(Name = "Kurs Açıklaması")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Fiyat (TL)")]
    public decimal Price { get; set; } = 0;

    [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
    [Display(Name = "Kategori")]
    public int CategoryId { get; set; }

    [Display(Name = "Kapak Görseli")]
    public IFormFile? CoverImage { get; set; }
}