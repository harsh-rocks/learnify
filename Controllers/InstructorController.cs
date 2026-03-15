using ELearningPlatform.Data;
using ELearningPlatform.Models;
using ELearningPlatform.Models.ViewModels;
using ELearningPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ELearningPlatform.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorageService _fileService;

        public InstructorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IFileStorageService fileService)
        {
            _context = context;
            _userManager = userManager;
            _fileService = fileService;
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new InstructorProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                ExistingProfilePictureUrl = user.ProfilePictureUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(InstructorProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();

                if (model.ProfilePicture != null)
                {
                    if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                    {
                        _fileService.DeleteFile(user.ProfilePictureUrl);
                    }
                    user.ProfilePictureUrl = await _fileService.SaveFileAsync(model.ProfilePicture, "avatars");
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Bio = model.Bio;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction(nameof(Profile));
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Students()
        {
            var user = await _userManager.GetUserAsync(User);
            // Get all enrollments for courses created by this instructor
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.Course.InstructorId == user.Id)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();

            return View(enrollments);
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var courses = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Lessons)
                .Include(c => c.Enrollments)
                .Where(c => c.InstructorId == user.Id)
                .ToListAsync();

            return View(courses);
        }

        public async Task<IActionResult> CreateCourse()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View(new CourseCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(CourseCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                
                string? imageUrl = null;
                if (model.ImageFile != null)
                {
                    imageUrl = await _fileService.SaveFileAsync(model.ImageFile, "course-images");
                }

                var course = new Course
                {
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price,
                    Level = model.Level,
                    Language = model.Language,
                    Duration = model.Duration,
                    IsPublished = model.IsPublished,
                    CategoryId = model.CategoryId,
                    InstructorId = user.Id,
                    ImageUrl = imageUrl
                };

                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", model.CategoryId);
            return View(model);
        }

        public async Task<IActionResult> EditCourse(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id && c.InstructorId == user.Id);
            if (course == null) return NotFound();

            var model = new CourseEditViewModel
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                CategoryId = course.CategoryId,
                Price = course.Price,
                Level = course.Level,
                Language = course.Language,
                Duration = course.Duration,
                IsPublished = course.IsPublished,
                ExistingImageUrl = course.ImageUrl
            };

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", course.CategoryId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(CourseEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == model.Id && c.InstructorId == user.Id);
                
                if (course == null) return NotFound();

                if (model.ImageFile != null)
                {
                    if (course.ImageUrl != null)
                    {
                        _fileService.DeleteFile(course.ImageUrl);
                    }
                    course.ImageUrl = await _fileService.SaveFileAsync(model.ImageFile, "course-images");
                }

                course.Title = model.Title;
                course.Description = model.Description;
                course.CategoryId = model.CategoryId;
                course.Price = model.Price;
                course.Level = model.Level;
                course.Language = model.Language;
                course.Duration = model.Duration;
                course.IsPublished = model.IsPublished;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", model.CategoryId);
            return View(model);
        }

        public async Task<IActionResult> ManageLessons(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            var course = await _context.Courses
                .Include(c => c.Sections.OrderBy(s => s.Order))
                    .ThenInclude(s => s.Lessons.OrderBy(l => l.OrderIndex))
                .FirstOrDefaultAsync(c => c.Id == courseId && c.InstructorId == user.Id);

            if (course == null) return NotFound();

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSection(int courseId, string title, int order)
        {
            var user = await _userManager.GetUserAsync(User);
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId && c.InstructorId == user.Id);
            
            if (course == null || string.IsNullOrWhiteSpace(title)) return BadRequest();

            var section = new Section
            {
                CourseId = courseId,
                Title = title,
                Order = order
            };

            _context.Sections.Add(section);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(ManageLessons), new { courseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSection(int id, int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            var section = await _context.Sections
                .Include(s => s.Course)
                .Include(s => s.Lessons)
                .FirstOrDefaultAsync(s => s.Id == id && s.Course.InstructorId == user.Id);

            if (section != null)
            {
                // Delete all lesson files inside this section
                foreach (var lesson in section.Lessons)
                {
                    if (lesson.VideoUrl != null) _fileService.DeleteFile(lesson.VideoUrl);
                    if (lesson.DocumentUrl != null) _fileService.DeleteFile(lesson.DocumentUrl);
                }

                _context.Sections.Remove(section);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(ManageLessons), new { courseId });
        }

        public async Task<IActionResult> AddLesson(int courseId, int? sectionId = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var course = await _context.Courses
                .Include(c => c.Sections)
                .FirstOrDefaultAsync(c => c.Id == courseId && c.InstructorId == user.Id);
                
            if (course == null) return NotFound();

            ViewBag.Sections = new SelectList(course.Sections.OrderBy(s => s.Order), "Id", "Title", sectionId);
            return View(new LessonCreateViewModel { CourseId = courseId, SectionId = sectionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(100_000_000)] // 100MB limit for video uploads (Warning: check IIS/Kestrel limits too)
        public async Task<IActionResult> AddLesson(LessonCreateViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var course = await _context.Courses
                .Include(c => c.Sections)
                .FirstOrDefaultAsync(c => c.Id == model.CourseId && c.InstructorId == user.Id);
            
            if (course == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                string? finalVideoUrl = model.VideoUrl;
                string? docUrl = null;

                if (model.VideoFile != null)
                {
                    finalVideoUrl = await _fileService.SaveFileAsync(model.VideoFile, "lesson-videos");
                }

                if (model.DocumentFile != null)
                    docUrl = await _fileService.SaveFileAsync(model.DocumentFile, "lesson-docs");

                var lesson = new Lesson
                {
                    CourseId = model.CourseId,
                    SectionId = model.SectionId,
                    Title = model.Title,
                    Description = model.Description,
                    Content = model.Content,
                    OrderIndex = model.OrderIndex,
                    Duration = model.Duration,
                    IsPreview = model.IsPreview,
                    VideoUrl = finalVideoUrl,
                    DocumentUrl = docUrl
                };

                _context.Add(lesson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageLessons), new { courseId = model.CourseId });
            }
            
            ViewBag.Sections = new SelectList(course.Sections.OrderBy(s => s.Order), "Id", "Title", model.SectionId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var course = await _context.Courses
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id && c.InstructorId == user.Id);
            
            if (course != null)
            {
                // Clean up files
                if (course.ImageUrl != null) _fileService.DeleteFile(course.ImageUrl);
                foreach (var lesson in course.Lessons)
                {
                    if (lesson.VideoUrl != null) _fileService.DeleteFile(lesson.VideoUrl);
                    if (lesson.DocumentUrl != null) _fileService.DeleteFile(lesson.DocumentUrl);
                }

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Dashboard));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLesson(int id, int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == id && l.Course.InstructorId == user.Id);
            
            if (lesson != null)
            {
                if (lesson.VideoUrl != null) _fileService.DeleteFile(lesson.VideoUrl);
                if (lesson.DocumentUrl != null) _fileService.DeleteFile(lesson.DocumentUrl);

                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageLessons), new { courseId = courseId });
        }

        // ──────────────────────────────────────────────────────────
        //  QUIZ BUILDER
        // ──────────────────────────────────────────────────────────

        public async Task<IActionResult> ManageQuiz(int lessonId)
        {
            var user = await _userManager.GetUserAsync(User);

            // Verify instructor owns this lesson's course
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .Include(l => l.Quiz)
                    .ThenInclude(q => q!.Questions)
                        .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(l => l.Id == lessonId && l.Course.InstructorId == user!.Id);

            if (lesson == null) return NotFound();

            ViewBag.Lesson = lesson;
            return View(lesson.Quiz);
        }

        public async Task<IActionResult> CreateQuiz(int lessonId)
        {
            var user = await _userManager.GetUserAsync(User);
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == lessonId && l.Course.InstructorId == user!.Id);

            if (lesson == null) return NotFound();

            ViewBag.LessonTitle = lesson.Title;
            return View(new QuizCreateViewModel { LessonId = lessonId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuiz(QuizCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var lesson = await _context.Lessons
                    .Include(l => l.Course)
                    .FirstOrDefaultAsync(l => l.Id == model.LessonId && l.Course.InstructorId == user!.Id);

                if (lesson == null) return NotFound();

                var quiz = new Quiz
                {
                    LessonId = model.LessonId,
                    Title = model.Title,
                    Description = model.Description
                };

                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageQuiz), new { lessonId = model.LessonId });
            }
            return View(model);
        }

        public async Task<IActionResult> AddQuestion(int quizId)
        {
            var user = await _userManager.GetUserAsync(User);
            var quiz = await _context.Quizzes
                .Include(q => q.Lesson).ThenInclude(l => l.Course)
                .FirstOrDefaultAsync(q => q.Id == quizId && q.Lesson.Course.InstructorId == user!.Id);

            if (quiz == null) return NotFound();

            ViewBag.QuizTitle = quiz.Title;
            ViewBag.LessonId  = quiz.LessonId;
            return View(new QuestionCreateViewModel { QuizId = quizId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestion(QuestionCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var quiz = await _context.Quizzes
                    .Include(q => q.Lesson).ThenInclude(l => l.Course)
                    .FirstOrDefaultAsync(q => q.Id == model.QuizId && q.Lesson.Course.InstructorId == user!.Id);

                if (quiz == null) return NotFound();

                var question = new Question
                {
                    QuizId = model.QuizId,
                    Text   = model.Text,
                    Points = model.Points
                };

                // Build options; mark the correct one
                var options = new List<Option>
                {
                    new Option { Text = model.OptionA, IsCorrect = model.CorrectOption == "A" },
                    new Option { Text = model.OptionB, IsCorrect = model.CorrectOption == "B" }
                };
                if (!string.IsNullOrWhiteSpace(model.OptionC))
                    options.Add(new Option { Text = model.OptionC, IsCorrect = model.CorrectOption == "C" });
                if (!string.IsNullOrWhiteSpace(model.OptionD))
                    options.Add(new Option { Text = model.OptionD, IsCorrect = model.CorrectOption == "D" });

                question.Options = options;
                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ManageQuiz), new { lessonId = quiz.LessonId });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestion(int id, int lessonId)
        {
            var user = await _userManager.GetUserAsync(User);
            var question = await _context.Questions
                .Include(q => q.Quiz).ThenInclude(q => q.Lesson).ThenInclude(l => l.Course)
                .FirstOrDefaultAsync(q => q.Id == id && q.Quiz.Lesson.Course.InstructorId == user!.Id);

            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageQuiz), new { lessonId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuiz(int id, int lessonId)
        {
            var user = await _userManager.GetUserAsync(User);
            var quiz = await _context.Quizzes
                .Include(q => q.Lesson).ThenInclude(l => l.Course)
                .FirstOrDefaultAsync(q => q.Id == id && q.Lesson.Course.InstructorId == user!.Id);

            if (quiz != null)
            {
                _context.Quizzes.Remove(quiz);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageQuiz), new { lessonId });
        }
    }
}
