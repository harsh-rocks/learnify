namespace ELearningPlatform.Models
{
    public class QuizAttempt
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public int QuizId { get; set; }
        public int Score { get; set; }
        public DateTime AttemptDate { get; set; } = DateTime.UtcNow;

        public virtual ApplicationUser Student { get; set; } = null!;
        public virtual Quiz Quiz { get; set; } = null!;
    }
}
