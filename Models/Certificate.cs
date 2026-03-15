using System.ComponentModel.DataAnnotations;

namespace ELearningPlatform.Models
{
    public class Certificate
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        public int CourseId { get; set; }
        public virtual Course Course { get; set; } = null!;

        [Required]
        public string StudentName { get; set; } = string.Empty;

        [Required]
        public string CourseTitle { get; set; } = string.Empty;

        [Required]
        public string InstructorName { get; set; } = string.Empty;

        public DateTime CompletionDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string CertificateId { get; set; } = Guid.NewGuid().ToString();
    }
}
