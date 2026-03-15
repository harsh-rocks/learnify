using ELearningPlatform.Data;
using ELearningPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELearningPlatform.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CourseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string query, int? categoryId)
        {
            var courses = _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                courses = courses.Where(c => c.Title.Contains(query) || c.Description.Contains(query));
                ViewBag.Query = query;
            }

            if (categoryId.HasValue)
            {
                courses = courses.Where(c => c.CategoryId == categoryId.Value);
                ViewBag.CategoryId = categoryId;
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(await courses.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Lessons.OrderBy(l => l.OrderIndex))
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return NotFound();

            bool isEnrolled = false;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                 if (user != null)
                 {
                     isEnrolled = await _context.Enrollments.AnyAsync(e => e.CourseId == id && e.StudentId == user.Id);
                 }
            }
            ViewBag.IsEnrolled = isEnrolled;

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int id)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                // Redirect to login, then come back to this course page automatically
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Details", "Course", new { id }) });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Details", "Course", new { id }) });
            }

            var alreadyEnrolled = await _context.Enrollments.AnyAsync(e => e.CourseId == id && e.StudentId == user.Id);

            if (!alreadyEnrolled)
            {
                var enrollment = new Enrollment
                {
                    CourseId = id,
                    StudentId = user.Id,
                    EnrollmentDate = DateTime.UtcNow,
                    ProgressPercentage = 0
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyCourse", "Student", new { id });
        }
    }
}
