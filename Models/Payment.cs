using System.ComponentModel.DataAnnotations;

namespace ELearningPlatform.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        public int CourseId { get; set; }
        public virtual Course Course { get; set; } = null!;

        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Success, Failed

        public string? TransactionId { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}
