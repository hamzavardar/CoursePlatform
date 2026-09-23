using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursePlatform.Data;
using CoursePlatform.Models;
using CoursePlatform.ViewModels;

namespace CoursePlatform.Controllers;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // 1. Kayıtlı Kurslar
    public async Task<IActionResult> MyCourses()
    {
        var studentId = _userManager.GetUserId(User);

        var enrolledCourses = await _context.Enrollments
            .Where(e => e.StudentId == studentId)
            .Include(e => e.Course)
                .ThenInclude(c => c.Teacher)
            .Include(e => e.Course)
                .ThenInclude(c => c.Lessons)
            .Select(e => e.Course)
            .ToListAsync();

        return View(enrolledCourses);
    }

    // 2. Kursa Kayıt Olma
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int courseId)
    {
        var studentId = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(studentId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var existingEnrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

        if (existingEnrollment == null)
        {
            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrolledAt = DateTime.Now
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(MyCourses));
    }

    // 3. Kurs İzleme ve İlerleme Takibi Sayfası
    public async Task<IActionResult> Watch(int courseId, int? lessonId)
    {
        var studentId = _userManager.GetUserId(User);

        var isEnrolled = await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

        if (!isEnrolled)
        {
            return RedirectToAction(nameof(MyCourses));
        }

        var course = await _context.Courses
            .Include(c => c.Teacher)
            .Include(c => c.Lessons.OrderBy(l => l.Order))
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null || !course.Lessons.Any())
        {
            TempData["ErrorMessage"] = "Bu kursta henüz ders bulunmamaktadır.";
            return RedirectToAction(nameof(MyCourses));
        }

        var currentLesson = lessonId.HasValue
            ? course.Lessons.FirstOrDefault(l => l.Id == lessonId.Value)
            : course.Lessons.First();

        if (currentLesson == null)
        {
            currentLesson = course.Lessons.First();
        }

        // Öğrencinin bu kursta tamamladığı ders ID'lerini çekme
        var lessonIds = course.Lessons.Select(l => l.Id).ToList();
        var completedLessonIds = await _context.LessonProgresses
            .Where(lp => lp.StudentId == studentId && lessonIds.Contains(lp.LessonId) && lp.IsCompleted)
            .Select(lp => lp.LessonId)
            .ToListAsync();

        // Tamamlanma yüzdesi hesaplama
        int totalLessons = course.Lessons.Count;
        int completedCount = completedLessonIds.Count;
        int progressPercentage = totalLessons > 0 ? (int)Math.Round((double)completedCount / totalLessons * 100) : 0;

        var viewModel = new CourseWatchViewModel
        {
            Course = course,
            CurrentLesson = currentLesson,
            Lessons = course.Lessons.ToList(),
            CompletedLessonIds = completedLessonIds,
            ProgressPercentage = progressPercentage
        };

        return View(viewModel);
    }

    // 4. Dersi Tamamlandı / Tamamlanmadı Olarak İşaretleme (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLessonProgress(int lessonId, int courseId)
    {
        var studentId = _userManager.GetUserId(User);

        var progress = await _context.LessonProgresses
            .FirstOrDefaultAsync(lp => lp.StudentId == studentId && lp.LessonId == lessonId);

        if (progress == null)
        {
            progress = new LessonProgress
            {
                StudentId = studentId!,
                LessonId = lessonId,
                IsCompleted = true,
                CompletedAt = DateTime.Now
            };
            _context.LessonProgresses.Add(progress);
        }
        else
        {
            progress.IsCompleted = !progress.IsCompleted;
            progress.CompletedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Watch), new { courseId = courseId, lessonId = lessonId });
    }
}