using System.ComponentModel.DataAnnotations;

namespace ELearningPlatform.Models.Entities
{
    public class Lesson
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? VideoUrl { get; set; }
        public string? DocumentUrl { get; set; }
        
        public int Order { get; set; }

        public int CourseId { get; set; }
        public virtual Course? Course { get; set; }
    }
}
