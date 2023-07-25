using CustomerApplication.Models;
using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.ViewModels;

public class AccountViewModel
{
    [Required]
    [Range(1000, 9999)]
    [Display(Name = "Account number")]
    public required int AccountNumber { get; init; }

    [Required]
    [Display(Name = "Account type")]
    public required AccountType AccountType { get; init; }

    [Required]
    [DataType(DataType.Currency)]
    public required decimal Balance { get; init; }

    [Required]
    [Display(Name = "Available")]
    [DataType(DataType.Currency)]
    public required decimal AvailableBalance { get; init; }
}