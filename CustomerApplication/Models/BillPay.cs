using CustomerApplication.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerApplication.Models;

public class BillPay
{
    public int BillPayID { get; set; }

    [Required]
    [ForeignKey("Account")]
    [Range(1000, 9999)]
    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }

    [Required]
    [ForeignKey("Payee")]
    public int PayeeID { get; set; }
    public virtual Payee Payee { get; set; }

    [Required]
    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    [CustomValidation(typeof(ValidationMethods), "MoreThanTwoDecimalPlaces")]
    [CustomValidation(typeof(ValidationMethods), "GreaterThanZero")]
    public decimal Amount { get; set; }

    [Required]
    [DataType(DataType.Date)]
    //[CustomValidation(typeof(ValidationMethods), "DateIsTenMinsFromNow")]
    public DateTime ScheduledTimeUtc { get; set; }

    [Required]
    public Period Period { get; set; }

    [Required]
    public BillPayStatus BillPayStatus { get; set; }
}