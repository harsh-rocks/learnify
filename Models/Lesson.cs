namespace ELearningPlatform.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int? SectionId { get; set; }
        public virtual Section? Section { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; } // Text lesson support
        public string? Description { get; set; }
        public string? VideoUrl { get; set; }
        public string? DocumentUrl { get; set; }
        public int Duration { get; set; } // in minutes
        public int OrderIndex { get; set; }
        public bool IsPreview { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Course Course { get; set; } = null!;
        public virtual Quiz? Quiz { get; set; }
    }
}
