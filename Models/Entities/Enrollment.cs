using System.ComponentModel.DataAnnotations;

namespace ELearningPlatform.Models.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        public double ProgressPercent { get; set; } = 0;

        public string StudentId { get; set; } = string.Empty;
        public virtual ApplicationUser? Student { get; set; }

        public int CourseId { get; set; }
        public virtual Course? Course { get; set; }
    }
}
