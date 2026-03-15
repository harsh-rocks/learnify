using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELearningPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly Data.ApplicationDbContext _context;

        public HomeController(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featuredCourses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .OrderByDescending(c => c.CreatedAt)
                .Take(4)
                .ToListAsync();

            return View(featuredCourses);
        }

        public IActionResult Courses()
        {
            return RedirectToAction("Index", "Course");
        }
    }
}
