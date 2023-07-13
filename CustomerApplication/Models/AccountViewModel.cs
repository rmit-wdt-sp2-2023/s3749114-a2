using BankLibrary.Models;
using BankLibrary.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerApplication.Models;

public class AccountViewModel
{
    [Required]
    [Display(Name = "Account number")]
    public int AccountNumber { get; set; }

    [Required]
    [Display(Name = "Account type")]
    public AccountType AccountType { get; set; }

    [DataType(DataType.Currency)]
    public required decimal Balance { get; init; }
}