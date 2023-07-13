using BankLibrary.Models;
using System.ComponentModel.DataAnnotations;
using BankLibrary.Validation;

namespace CustomerApplication.Models;

public class TransactionViewModel
{
    [Display(Name = "Account Number")]
    public int AccountNumber { get; set; }

    [Display(Name = "Account Type")]
    public AccountType AccountType { get; set; }

    [DataType(DataType.Currency)]
    public decimal Balance { get; set; }

    [Required]
    [CustomValidation(typeof(ValidationMethods), "MoreThanTwoDecimalPlaces")]
    [CustomValidation(typeof(ValidationMethods), "GreaterThanZero")]
    public decimal Amount { get; set; }

    [StringLength(30, ErrorMessage = "Comment cannot be more than 40 characters")]
    public string Comment { get; set; }

    [Display(Name = "TransactionType")]
    public required TransactionType TransactionType { get; init; }
}