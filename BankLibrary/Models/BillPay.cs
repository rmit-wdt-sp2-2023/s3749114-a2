using BankLibrary.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankLibrary.Models;

public class BillPay
{
    [Display(Name = "ID")]
    public int BillPayID { get; set; }

    [Required]
    [ForeignKey("Account")]
    [Display(Name = "Account no.")]
    [Range(1000, 9999)]
    public required int AccountNumber { get; set; }
    public virtual Account Account { get; set; }

    [ForeignKey("Payee")]
    [Required(ErrorMessage = "You must enter a payee ID.")]
    [Display(Name = "Payee ID")]
    public required int PayeeID { get; set; }
    public virtual Payee Payee { get; set; }

    [Required(ErrorMessage = "You must enter an amount.")]
    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    [CustomValidation(typeof(ValidationMethods), "MoreThanTwoDecimalPlaces")]
    [CustomValidation(typeof(ValidationMethods), "GreaterThanZero")]
    public required decimal Amount { get; set; }

    [Required(ErrorMessage = "You must enter a time.")]
    [DataType(DataType.Date)]
    [Display(Name = "Scheduled time")]
    [CustomValidation(typeof(ValidationMethods), "IsTenMinsFromNowUtc")]
    public required DateTime ScheduledTimeUtc { get; set; }

    [Required]
    public required Period Period { get; set; }

    [Required]
    [Display(Name = "Status")]
    public required BillPayStatus BillPayStatus { get; set; }
}