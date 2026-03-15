using ELearningPlatform.Data;
using ELearningPlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELearningPlatform.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class QuizController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Manage(int lessonId)
        {
            var user = await _userManager.GetUserAsync(User);
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .Include(l => l.Quiz)
                    .ThenInclude(q => q.Questions)
                        .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(l => l.Id == lessonId && l.Course.InstructorId == user.Id);

            if (lesson == null) return NotFound();

            if (lesson.Quiz == null)
            {
                var quiz = new Quiz { LessonId = lessonId, Title = $"Quiz: {lesson.Title}" };
                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();
                
                lesson.Quiz = quiz;
            }

            return View(lesson.Quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuizInfo(int id, string title, string description)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Lesson)
                .ThenInclude(l => l.Course)
                .FirstOrDefaultAsync(q => q.Id == id);
                
            var user = await _userManager.GetUserAsync(User);

            if (quiz != null && quiz.Lesson.Course.InstructorId == user.Id)
            {
                quiz.Title = title;
                quiz.Description = description;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Manage), new { lessonId = quiz?.LessonId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestion(int quizId, string text, int points, string[] optionTexts, int correctOptionIndex)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Lesson)
                    .ThenInclude(l => l.Course)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            var user = await _userManager.GetUserAsync(User);
            if (quiz == null || quiz.Lesson.Course.InstructorId != user.Id) return Unauthorized();

            if (!string.IsNullOrWhiteSpace(text) && optionTexts != null && optionTexts.Length > 0)
            {
                var question = new Question
                {
                    QuizId = quizId,
                    Text = text,
                    Points = points
                };
                
                for (int i = 0; i < optionTexts.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(optionTexts[i]))
                    {
                        question.Options.Add(new Option
                        {
                            Text = optionTexts[i],
                            IsCorrect = (i == correctOptionIndex)
                        });
                    }
                }

                _context.Questions.Add(question);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Manage), new { lessonId = quiz.LessonId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestion(int id, int quizId)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Manage), new { lessonId = _context.Quizzes.Find(quizId)?.LessonId });
        }
    }
}
