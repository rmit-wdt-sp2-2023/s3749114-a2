using BankLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace AdminPortal.ViewModels;

public class LoginProfileViewModel
{
    [Required]
    [RegularExpression(@"^\d{8}$")]
    public required string LoginID { get; set; }

    [Required]
    [Range(1000, 9999)]
    [Display(Name = "Customer ID")]
    public required int CustomerID { get; set; }

    [Required]
    [Display(Name = "Login status")]
    public required LoginStatus LoginStatus { get; set; }
}