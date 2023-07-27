using BankLibrary.Models;
using BankLibrary.Validation;
using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.ViewModels;

public class ReceiptViewModel
{
    [Required]
    [Display(Name = "Type")]
    public TransactionType TransactionType { get; set; }

    [Required]
    [Range(1000, 9999)]
    [Display(Name = "Account no.")]
    public int AccountNumber { get; set; }

    [Range(1000, 9999)]
    [Display(Name = "Destination no.")]
    public int? DestinationNumber { get; set; } = null;

    [Required]
    [CustomValidation(typeof(ValidationMethods), "MoreThanTwoDecimalPlaces")]
    [CustomValidation(typeof(ValidationMethods), "GreaterThanZero")]
    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    [Display(Name = "Service charge")]
    [DataType(DataType.Currency)]
    public decimal? ServiceCharge { get; set; }

    [StringLength(30)]
    public string Comment { get; set; }

    [Required]
    [Display(Name = "Time")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
    public DateTime TransactionTimeLocal { get; set; }
}