using System.ComponentModel.DataAnnotations;

namespace AdminPortal.ViewModels;

public class LoginSearchViewModel
{
    [StringLength(8)]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "Must have exactly 8 digits.")]
    public string LoginID { get; set; }
}