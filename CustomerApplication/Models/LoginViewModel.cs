using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.Models;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Login ID")]
    public string LoginID { get; set; }

    [Required]
    public string Password { get; set; }
}