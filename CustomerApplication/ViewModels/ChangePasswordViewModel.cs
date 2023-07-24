using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.ViewModels;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "You must enter your old password.")]
    [Display(Name = "Old password")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "You must enter a new password.")]
    [Display(Name = "New password")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "You must confirm your new password.")]
    [Display(Name = "Confirm password")]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords don't match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}