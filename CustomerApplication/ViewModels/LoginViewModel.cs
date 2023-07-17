using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.ViewModels;

public class LoginViewModel
{
    [Required (ErrorMessage = "You must enter a login ID.")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "Invalid login ID.")]
    [Display(Name = "Login ID")]
    public string LoginID { get; set; }

    [Required (ErrorMessage = "You must enter a password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}