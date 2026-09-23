using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursePlatform.Data;
using CoursePlatform.Models;
using CoursePlatform.ViewModels;


namespace CoursePlatform.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Öne çıkan / En son eklenen 6 kurs
        var featuredCourses = await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Teacher)
            .Include(c => c.Lessons)
            .OrderByDescending(c => c.CreatedAt)
            .Take(6)
            .ToListAsync();

        // Platform istatistikleri
        ViewBag.TotalCourses = await _context.Courses.CountAsync();
        ViewBag.TotalLessons = await _context.Lessons.CountAsync();
        ViewBag.TotalStudents = await _context.Enrollments.Select(e => e.StudentId).Distinct().CountAsync();

        return View(featuredCourses);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}