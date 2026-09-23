using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoursePlatform.Data;
using CoursePlatform.Models;
using CoursePlatform.Services;
using CoursePlatform.ViewModels;

namespace CoursePlatform.Controllers;

[Authorize(Roles = "Teacher")]
public class TeacherController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;

    public TeacherController(ApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<IActionResult> Index()
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var courses = await _context.Courses
            .AsNoTracking()
            .Where(c => c.TeacherId == teacherId)
            .Include(c => c.Category)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return View(courses);
    }

    public async Task<IActionResult> CreateCourse()
    {
        var categories = await _context.Categories.AsNoTracking().ToListAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCourse(CreateCourseViewModel model)
    {
        string? coverImagePath = null;

        if (model.CoverImage != null)
        {
            var uploadResult = await _fileService.UploadImageAsync(model.CoverImage);
            if (!uploadResult.IsValid)
            {
                ModelState.AddModelError("CoverImage", uploadResult.ErrorMessage!);
            }
            else
            {
                coverImagePath = uploadResult.FilePath;
            }
        }

        if (ModelState.IsValid)
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var course = new Course
            {
                Title = model.Title.Trim(),
                Description = model.Description.Trim(),
                Price = 0,
                CategoryId = model.CategoryId,
                TeacherId = teacherId!,
                CoverImageUrl = coverImagePath,
                CreatedAt = DateTime.Now
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = new SelectList(await _context.Categories.AsNoTracking().ToListAsync(), "Id", "Name", model.CategoryId);
        return View(model);
    }

    public async Task<IActionResult> EditCourse(int id)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id && c.TeacherId == teacherId);

        if (course == null) return NotFound();

        var viewModel = new EditCourseViewModel
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            CategoryId = course.CategoryId,
            CurrentCoverImageUrl = course.CoverImageUrl
        };

        ViewBag.Categories = new SelectList(await _context.Categories.AsNoTracking().ToListAsync(), "Id", "Name", course.CategoryId);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCourse(EditCourseViewModel model)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == model.Id && c.TeacherId == teacherId);

        if (course == null) return NotFound();

        if (model.NewCoverImage != null)
        {
            var uploadResult = await _fileService.UploadImageAsync(model.NewCoverImage);
            if (!uploadResult.IsValid)
            {
                ModelState.AddModelError("NewCoverImage", uploadResult.ErrorMessage!);
            }
            else
            {
                if (!string.IsNullOrEmpty(course.CoverImageUrl))
                    _fileService.DeleteFile(course.CoverImageUrl);

                course.CoverImageUrl = uploadResult.FilePath;
            }
        }

        if (ModelState.IsValid)
        {
            course.Title = model.Title.Trim();
            course.Description = model.Description.Trim();
            course.CategoryId = model.CategoryId;

            _context.Courses.Update(course);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CourseDetails), new { id = course.Id });
        }

        ViewBag.Categories = new SelectList(await _context.Categories.AsNoTracking().ToListAsync(), "Id", "Name", model.CategoryId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var course = await _context.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id && c.TeacherId == teacherId);

        if (course != null)
        {
            if (!string.IsNullOrEmpty(course.CoverImageUrl))
                _fileService.DeleteFile(course.CoverImageUrl);

            foreach (var lesson in course.Lessons)
            {
                _fileService.DeleteFile(lesson.VideoUrl);
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> AddLesson(int courseId)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == courseId && c.TeacherId == teacherId);

        if (course == null) return NotFound();

        var nextOrder = await _context.Lessons.AsNoTracking().Where(l => l.CourseId == courseId).CountAsync() + 1;

        var viewModel = new AddLessonViewModel
        {
            CourseId = course.Id,
            CourseTitle = course.Title,
            Order = nextOrder
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddLesson(AddLessonViewModel model)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == model.CourseId && c.TeacherId == teacherId);

        if (course == null) return NotFound();

        string? videoPath = null;
        var uploadResult = await _fileService.UploadVideoAsync(model.VideoFile);
        if (!uploadResult.IsValid)
        {
            ModelState.AddModelError("VideoFile", uploadResult.ErrorMessage!);
        }
        else
        {
            videoPath = uploadResult.FilePath;
        }

        if (ModelState.IsValid)
        {
            var lesson = new Lesson
            {
                CourseId = model.CourseId,
                Title = model.Title.Trim(),
                Description = model.Description?.Trim() ?? "",
                Order = model.Order,
                VideoUrl = videoPath!
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CourseDetails), new { id = model.CourseId });
        }

        model.CourseTitle = course.Title;
        return View(model);
    }

    public async Task<IActionResult> EditLesson(int id)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var lesson = await _context.Lessons
            .AsNoTracking()
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => l.Id == id && l.Course.TeacherId == teacherId);

        if (lesson == null) return NotFound();

        var viewModel = new EditLessonViewModel
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Description = lesson.Description,
            Order = lesson.Order,
            CurrentVideoUrl = lesson.VideoUrl
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditLesson(EditLessonViewModel model)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var lesson = await _context.Lessons
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => l.Id == model.Id && l.Course.TeacherId == teacherId);

        if (lesson == null) return NotFound();

        if (model.NewVideoFile != null)
        {
            var uploadResult = await _fileService.UploadVideoAsync(model.NewVideoFile);
            if (!uploadResult.IsValid)
            {
                ModelState.AddModelError("NewVideoFile", uploadResult.ErrorMessage!);
            }
            else
            {
                _fileService.DeleteFile(lesson.VideoUrl);
                lesson.VideoUrl = uploadResult.FilePath!;
            }
        }

        if (ModelState.IsValid)
        {
            lesson.Title = model.Title.Trim();
            lesson.Description = model.Description?.Trim() ?? "";
            lesson.Order = model.Order;

            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CourseDetails), new { id = lesson.CourseId });
        }

        model.CurrentVideoUrl = lesson.VideoUrl;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLesson(int id, int courseId)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var lesson = await _context.Lessons
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => l.Id == id && l.Course.TeacherId == teacherId);

        if (lesson != null)
        {
            _fileService.DeleteFile(lesson.VideoUrl);
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(CourseDetails), new { id = courseId });
    }

    public async Task<IActionResult> CourseDetails(int id)
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var course = await _context.Courses
            .AsNoTracking()
            .AsSplitQuery()
            .Include(c => c.Category)
            .Include(c => c.Lessons.OrderBy(l => l.Order))
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id && c.TeacherId == teacherId);

        if (course == null) return NotFound();

        return View(course);
    }
}