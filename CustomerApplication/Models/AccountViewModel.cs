using BankLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.Models;

public class AccountViewModel
{
    [Required]
    [Display(Name = "Account number")]
    public int AccountNumber { get; set; }

    [Required]
    [Display(Name = "Account type")]
    public AccountType AccountType { get; set; }
}