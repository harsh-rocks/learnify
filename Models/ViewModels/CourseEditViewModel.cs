using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ELearningPlatform.Models.ViewModels
{
    public class CourseEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        [StringLength(50)]
        [Display(Name = "Difficulty Level")]
        public string? Level { get; set; }

        [StringLength(50)]
        public string? Language { get; set; }

        [Display(Name = "Estimated Duration (minutes)")]
        public int Duration { get; set; }

        [Display(Name = "Publish Course?")]
        public bool IsPublished { get; set; }

        [Display(Name = "Update Course Thumbnail Image")]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImageUrl { get; set; }
    }
}
