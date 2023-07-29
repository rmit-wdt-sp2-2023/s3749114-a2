using System.ComponentModel.DataAnnotations;

namespace AdminPortal.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "You must enter a username.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "You must enter a password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}