using System.ComponentModel.DataAnnotations;

namespace ELearningPlatform.Models
{
    public class Section
    {
        public int Id { get; set; }
        
        public int CourseId { get; set; }
        public virtual Course Course { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public int Order { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
