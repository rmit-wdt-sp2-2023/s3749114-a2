using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.ViewModels;

public class ChangePasswordViewModel
{
    [Required]
    [Display(Name = "Old password")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Required]
    [Display(Name = "New password")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required]
    [Display(Name = "Confirm password")]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords don't match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}