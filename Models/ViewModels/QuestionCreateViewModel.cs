using System.ComponentModel.DataAnnotations;

namespace ELearningPlatform.Models.ViewModels
{
    public class QuestionCreateViewModel
    {
        public int QuizId { get; set; }

        [Required]
        [Display(Name = "Question Text")]
        public string Text { get; set; } = string.Empty;

        [Display(Name = "Points")]
        [Range(1, 100)]
        public int Points { get; set; } = 1;

        // Up to 4 answer options; first one marked correct
        [Required]
        [Display(Name = "Option A (Correct Answer)")]
        public string OptionA { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Option B")]
        public string OptionB { get; set; } = string.Empty;

        [Display(Name = "Option C")]
        public string? OptionC { get; set; }

        [Display(Name = "Option D")]
        public string? OptionD { get; set; }

        // Which option is correct: "A", "B", "C", or "D"
        [Required]
        [Display(Name = "Correct Answer")]
        public string CorrectOption { get; set; } = "A";
    }
}
