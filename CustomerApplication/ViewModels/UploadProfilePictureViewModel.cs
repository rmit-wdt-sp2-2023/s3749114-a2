using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.ViewModels;

public class UploadProfilePictureViewModel
{
    [Display(Name = "Select profile image")]
    [Required(ErrorMessage = "You must select an image.")]
    public IFormFile ProfileImage { get; set; }
}