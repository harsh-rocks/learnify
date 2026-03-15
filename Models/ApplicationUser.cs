using Microsoft.AspNetCore.Identity;

namespace ELearningPlatform.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        
        // Instructor Profile fields
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        
        // Navigation properties
        public virtual ICollection<Course> InstructedCourses { get; set; } = new List<Course>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    }
}
