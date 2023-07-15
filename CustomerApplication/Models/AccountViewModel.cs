using BankLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.Models;

public class AccountViewModel
{
    [Required]
    [Range(1000, 9999)]
    [Display(Name = "Account Number")]
    public int AccountNumber { get; set; }

    [Required]
    [Display(Name = "Account Type")]
    public AccountType AccountType { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    public required decimal Balance { get; init; }

    [Required]
    [Display(Name = "Available Balance")]
    [DataType(DataType.Currency)]
    public required decimal AvailableBalance { get; init; }
}