using CustomerApplication.Models;
using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.ViewModels;

public class AccountViewModel
{
    [Required]
    [Range(1000, 9999)]
    [Display(Name = "Account number")]
    public int AccountNumber { get; set; }

    [Required]
    [Display(Name = "Account type")]
    public AccountType AccountType { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    public decimal Balance { get; init; }

    [Required]
    [Display(Name = "Available balance")]
    [DataType(DataType.Currency)]
    public decimal AvailableBalance { get; init; }
}