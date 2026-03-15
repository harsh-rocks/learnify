using System.ComponentModel.DataAnnotations;

namespace ELearningPlatform.Models.ViewModels
{
    public class QuizCreateViewModel
    {
        public int LessonId { get; set; }

        [Required]
        [Display(Name = "Quiz Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Description (optional)")]
        public string? Description { get; set; }
    }
}
