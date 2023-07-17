using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.ViewModels;

public class ChangePasswordViewModel
{
    [Required]
    [Display(Name = "Old Password")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Required]
    [Display(Name = "New Password")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required]
    [Display(Name = "Confirm Password")]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords don't match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

    public bool PasswordUpdated { get; set; } = false;
}