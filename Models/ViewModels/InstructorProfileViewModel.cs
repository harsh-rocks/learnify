using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ELearningPlatform.Models.ViewModels
{
    public class InstructorProfileViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Professional Biography")]
        public string? Bio { get; set; }

        public string? ExistingProfilePictureUrl { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }
    }
}
