using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.Models;

public class ChangePasswordViewModel
{
    [Required]
    [Display(Name = "Old Password")]
    public string OldPassword { get; set; }

    [Required]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; }

    [Required]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; }

    public bool PasswordUpdated { get; set; } = false;
}