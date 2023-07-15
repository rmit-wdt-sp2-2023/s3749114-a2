using BankLibrary.Models;
using System.ComponentModel.DataAnnotations;
using BankLibrary.Validation;

namespace CustomerApplication.Models;

public class TransactionViewModel
{
    [Required]
    [Display(Name = "Account Number")]
    [Range(1000, 9999, ErrorMessage = "You must select an Account.")]
    public int AccountNumber { get; set; }

    [Required]
    [Display(Name = "Account Type")]
    public AccountType AccountType { get; set; }

    public List<AccountViewModel> AccountsViewModel { get; set; }

    [Display(Name = "Account Number")]
    [Range(1000, 9999, ErrorMessage = "You must enter a valid Account Number.")]
    public int? DestinationNumber { get; set; } = null;

    [Required]
    [CustomValidation(typeof(ValidationMethods), "MoreThanTwoDecimalPlaces")]
    [CustomValidation(typeof(ValidationMethods), "GreaterThanZero")]
    public decimal Amount { get; set; }

    [StringLength(30, ErrorMessage = "Comment cannot be more than 30 characters.")]
    public string Comment { get; set; } = null;

    [Required]
    [Display(Name = "Transaction Type")]
    public required TransactionType TransactionType { get; init; }

    public bool TransactionResult { get; set; } 
}