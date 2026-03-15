using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELearningPlatform.Models.Entities
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string? ThumbnailUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign keys
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public string InstructorId { get; set; } = string.Empty;
        public virtual ApplicationUser? Instructor { get; set; }

        // Navigation properties
        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }
}
