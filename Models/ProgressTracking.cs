using System.ComponentModel.DataAnnotations;

namespace ELearningPlatform.Models
{
    public class ProgressTracking
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        public int CourseId { get; set; }
        public virtual Course Course { get; set; } = null!;

        public int LessonId { get; set; }
        public virtual Lesson Lesson { get; set; } = null!;

        public bool IsCompleted { get; set; }
        public DateTime LastWatchedDate { get; set; } = DateTime.UtcNow;
    }
}
