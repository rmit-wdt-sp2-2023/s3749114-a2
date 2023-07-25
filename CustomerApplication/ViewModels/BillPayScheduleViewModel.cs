using System.ComponentModel.DataAnnotations;
using CustomerApplication.Models;
using CustomerApplication.Validation;

namespace CustomerApplication.ViewModels;

public class BillPayScheduleViewModel
{
    [Display(Name = "Account no.")]
    [Required(ErrorMessage = "You must select an account.")]
    public int? AccountNumber { get; set; }

    public AccountType AccountType { get; set; }

    [UIHint("AccountSelection")]
    public List<AccountViewModel> AccountViewModels { get; set; }

    [Required(ErrorMessage = "You must enter an amount.")]
    [CustomValidation(typeof(ValidationMethods), "MoreThanTwoDecimalPlaces")]
    [CustomValidation(typeof(ValidationMethods), "GreaterThanZero")]
    [DataType(DataType.Currency)]
    public decimal? Amount { get; set; }

    [Display(Name = "Scheduled time")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
    [Required(ErrorMessage = "You must enter a time.")]
    [CustomValidation(typeof(ValidationMethods), "IsTenMinsFromNowLocal")]
    public DateTime? ScheduledTimeLocal{ get; set; }

    [Required(ErrorMessage = "You must select a period.")]
    public Period? Period { get; set; }

    [Display(Name = "Payee ID")]
    [Required(ErrorMessage = "You must enter a payee ID.")]
    public int? PayeeID { get; set; }
}