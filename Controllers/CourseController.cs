using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursePlatform.Data;
using CoursePlatform.ViewModels;

namespace CoursePlatform.Controllers;

[AllowAnonymous]
public class CourseController : Controller
{
    private readonly ApplicationDbContext _context;

    public CourseController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? categoryId, string? searchString)
    {
        var query = _context.Courses
            .AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Teacher)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            var search = searchString.Trim();
            query = query.Where(c => c.Title.Contains(search) || c.Description.Contains(search));
        }

        ViewBag.Categories = await _context.Categories.AsNoTracking().ToListAsync();
        ViewBag.CurrentCategory = categoryId;
        ViewBag.SearchString = searchString;

        var courses = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        return View(courses);
    }

    public async Task<IActionResult> Details(int id)
    {
        var course = await _context.Courses
            .AsNoTracking()
            .AsSplitQuery()
            .Include(c => c.Category)
            .Include(c => c.Teacher)
            .Include(c => c.Lessons.OrderBy(l => l.Order))
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return NotFound();

        bool isEnrolled = false;
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            isEnrolled = course.Enrollments.Any(e => e.StudentId == userId);
        }

        var viewModel = new CourseDetailViewModel
        {
            Course = course,
            IsEnrolled = isEnrolled
        };

        return View(viewModel);
    }
}