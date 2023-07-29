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
    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }

    [ForeignKey("Payee")]
    [Required(ErrorMessage = "You must enter a payee ID.")]
    [Display(Name = "Payee ID")]
    public int PayeeID { get; set; }
    public virtual Payee Payee { get; set; }

    [Required(ErrorMessage = "You must enter an amount.")]
    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    [CustomValidation(typeof(ValidationMethods), "MoreThanTwoDecimalPlaces")]
    [CustomValidation(typeof(ValidationMethods), "GreaterThanZero")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "You must enter a time.")]
    [DataType(DataType.Date)]
    [Display(Name = "Scheduled time")]
    [CustomValidation(typeof(ValidationMethods), "IsTenMinsFromNowUtc")]
    public DateTime ScheduledTimeUtc { get; set; }

    [Required]
    public Period Period { get; set; }

    [Required]
    [Display(Name = "Status")]
    public BillPayStatus BillPayStatus { get; set; }
}