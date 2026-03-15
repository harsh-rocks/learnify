using ELearningPlatform.Data;
using ELearningPlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELearningPlatform.Controllers
{
    [Authorize(Roles = "Admin,Student,Instructor")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            
            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Instructor)
                .Where(e => e.StudentId == user.Id)
                .ToListAsync();

            return View(enrollments);
        }

        public async Task<IActionResult> MyCourse(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Lessons.OrderBy(l => l.OrderIndex))
                        .ThenInclude(l => l.Quiz)
                .FirstOrDefaultAsync(e => e.CourseId == id && e.StudentId == user.Id);

            if (enrollment == null) return Forbid(); // Not enrolled

            // Calculate progress (just a mock calculation based on quiz attempts or lessons viewed if we added tracking)
            var totalLessons = enrollment.Course.Lessons.Count;
            if (totalLessons > 0)
            {
                var quizAttempts = await _context.QuizAttempts
                    .Where(qa => qa.StudentId == user.Id && qa.Quiz.Lesson.CourseId == id)
                    .Select(qa => qa.Quiz.LessonId)
                    .Distinct()
                    .CountAsync();
                
                enrollment.ProgressPercentage = (decimal)quizAttempts / totalLessons * 100;
                await _context.SaveChangesAsync();
            }

            return View(enrollment);
        }

        public async Task<IActionResult> PlayLesson(int lessonId)
        {
            var user = await _userManager.GetUserAsync(User);
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .Include(l => l.Quiz)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null) return NotFound();

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == lesson.CourseId && e.StudentId == user.Id);
                
            if (enrollment == null) return Forbid(); // Must be enrolled to view

            // Find previous and next lessons for navigation
            ViewBag.NextLessonId = await _context.Lessons
                .Where(l => l.CourseId == lesson.CourseId && l.OrderIndex > lesson.OrderIndex)
                .OrderBy(l => l.OrderIndex)
                .Select(l => l.Id)
                .FirstOrDefaultAsync();
                
            ViewBag.PrevLessonId = await _context.Lessons
                .Where(l => l.CourseId == lesson.CourseId && l.OrderIndex < lesson.OrderIndex)
                .OrderByDescending(l => l.OrderIndex)
                .Select(l => l.Id)
                .FirstOrDefaultAsync();

            return View(lesson);
        }
        public async Task<IActionResult> TakeQuiz(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var quiz = await _context.Quizzes
                .Include(q => q.Lesson)
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null) return NotFound();

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == quiz.Lesson.CourseId && e.StudentId == user.Id);
                
            if (enrollment == null) return Forbid();

            // Shuffle options for better experience
            foreach (var q in quiz.Questions)
            {
                var shuffled = q.Options.OrderBy(x => Guid.NewGuid()).ToList();
                q.Options = shuffled;
            }

            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitQuiz(int id, IFormCollection form)
        {
            var user = await _userManager.GetUserAsync(User);
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null) return NotFound();

            int score = 0;
            int totalPoints = 0;

            foreach (var question in quiz.Questions)
            {
                totalPoints += question.Points;
                var selectedOptionIdStr = form[$"question_{question.Id}"];
                if (!string.IsNullOrEmpty(selectedOptionIdStr) && int.TryParse(selectedOptionIdStr, out int selectedOptionId))
                {
                    var selectedOption = question.Options.FirstOrDefault(o => o.Id == selectedOptionId);
                    if (selectedOption != null && selectedOption.IsCorrect)
                    {
                        score += question.Points;
                    }
                }
            }

            var attempt = new QuizAttempt
            {
                QuizId = id,
                StudentId = user.Id,
                Score = (int)Math.Round(totalPoints == 0 ? 0 : ((double)score / totalPoints) * 100),
                AttemptDate = DateTime.UtcNow
            };

            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(QuizResult), new { id = attempt.Id });
        }

        public async Task<IActionResult> QuizResult(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var attempt = await _context.QuizAttempts
                .Include(a => a.Quiz)
                    .ThenInclude(q => q.Lesson)
                .FirstOrDefaultAsync(a => a.Id == id && a.StudentId == user.Id);

            if (attempt == null) return NotFound();

            return View(attempt);
        }
    }
}
