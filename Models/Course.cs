using System.ComponentModel.DataAnnotations;

namespace ELearningPlatform.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public bool IsFree { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string? Level { get; set; }
        [StringLength(50)]
        public string? Language { get; set; }
        public int Duration { get; set; } // in minutes
        public bool IsPublished { get; set; }

        // Foreign keys
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        public string InstructorId { get; set; } = string.Empty;
        public virtual ApplicationUser Instructor { get; set; } = null!;

        public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
