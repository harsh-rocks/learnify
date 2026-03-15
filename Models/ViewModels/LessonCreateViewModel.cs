using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ELearningPlatform.Models.ViewModels
{
    public class LessonCreateViewModel
    {
        public int Id { get; set; }
        
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Please select a Section")]
        [Display(Name = "Section")]
        public int? SectionId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Content { get; set; }

        [Display(Name = "Duration (minutes)")]
        public int Duration { get; set; }

        [Display(Name = "Order Index (Sequence)")]
        public int OrderIndex { get; set; }

        [Display(Name = "Free Preview Lesson")]
        public bool IsPreview { get; set; }

        [Display(Name = "Video File")]
        public IFormFile? VideoFile { get; set; }

        [Display(Name = "YouTube Video URL")]
        public string? VideoUrl { get; set; }

        public string? ExistingVideoUrl { get; set; }

        [Display(Name = "Document/Resource File")]
        public IFormFile? DocumentFile { get; set; }

        public string? ExistingDocumentUrl { get; set; }
    }
}
