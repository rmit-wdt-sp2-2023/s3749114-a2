using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.Models;

public class LoginViewModel
{
    [Required]
    public string LoginID { get; set; }

    [Required]
    public string Password { get; set; }
}