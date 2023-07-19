using CustomerApplication.Models;
using System.ComponentModel.DataAnnotations;
using CustomerApplication.Validation;

namespace CustomerApplication.ViewModels;

public class TransactionViewModel
{
    [Required]
    [Display(Name = "Account number")]
    //[Range(1000, 9999, ErrorMessage = "You must select an account.")]
    public int AccountNumber { get; set; }

    [Required]
    [Display(Name = "Account type")]
    public AccountType AccountType { get; set; }

    public List<AccountViewModel> AccountsViewModel { get; set; }

    [Display(Name = "Account number")]
    //[Range(1000, 9999, ErrorMessage = "You must enter a valid 4 digit account number.")]
    public int? DestinationNumber { get; set; } = null;

    [Required]
    //[CustomValidation(typeof(ValidationMethods), "MoreThanTwoDecimalPlaces")]
    //[CustomValidation(typeof(ValidationMethods), "GreaterThanZero")]
    public decimal Amount { get; set; }

    //[StringLength(30, ErrorMessage = "Comment cannot be more than 30 characters.")]
    public string Comment { get; set; } = null;

    [Required]
    [Display(Name = "Transaction type")]
    public required TransactionType TransactionType { get; init; }
}