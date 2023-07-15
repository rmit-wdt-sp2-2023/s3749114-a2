using BankLibrary.Models;
using System.ComponentModel.DataAnnotations;
using BankLibrary.Validation;

namespace CustomerApplication.Models;

public class TransactionViewModel
{
    [Required]
    [Display(Name = "Account Number")]
    public int AccountNumber { get; set; }

    [Display(Name = "Account Type")]
    public AccountType AccountType { get; set; }

    public List<AccountViewModel> AccountsViewModel { get; set; }

    [Required]
    [CustomValidation(typeof(ValidationMethods), "MoreThanTwoDecimalPlaces")]
    [CustomValidation(typeof(ValidationMethods), "GreaterThanZero")]
    public decimal Amount { get; set; }

    [StringLength(30, ErrorMessage = "Comment cannot be more than 30 characters.")]
    public string Comment { get; set; }

    [Required]
    [Display(Name = "Transaction Type")]
    public required TransactionType TransactionType { get; init; }
}