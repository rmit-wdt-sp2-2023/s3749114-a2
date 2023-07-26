using CustomerApplication.Models;
using System.ComponentModel.DataAnnotations;
using CustomerApplication.Validation;

namespace CustomerApplication.ViewModels;

public class CreateTransactionViewModel
{
    [Required(ErrorMessage = "You must select an account.")]
    [Display(Name = "Account no.")]
    [Range(1000, 9999, ErrorMessage = "Invalid account number.")]
    public int? AccountNumber { get; set; }

    public List<AccountViewModel> AccountViewModels { get; set; } = new();

    [Display(Name = "Account no.")]
    //[Range(1000, 9999, ErrorMessage = "You must enter a valid 4 digit account number.")]
    public int? DestinationNumber { get; set; } = null;

    [Required(ErrorMessage = "You must enter an amount.")]
    [CustomValidation(typeof(ValidationMethods), "MoreThanTwoDecimalPlaces")]
    [CustomValidation(typeof(ValidationMethods), "GreaterThanZero")]
    [DataType(DataType.Currency)]
    public decimal? Amount { get; set; }

    [StringLength(30, ErrorMessage = "Comment cannot be more than 30 characters.")]
    public string Comment { get; set; } = null;

    [Required]
    [Display(Name = "Type")]
    public required TransactionType TransactionType { get; init; }
}