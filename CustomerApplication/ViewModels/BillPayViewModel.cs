using System.ComponentModel.DataAnnotations;
using CustomerApplication.Models;

namespace CustomerApplication.ViewModels;

public class BillPayViewModel
{
    [Display(Name = "ID")]
    public int BillPayID { get; set; }

    [Display(Name = "Account number")]
    public int AccountNumber { get; set; }

    [Display(Name = "Payee ID")]
    public int PayeeID { get; set; }

    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    [Display(Name = "Scheduled time")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
    public DateTime ScheduledTimeLocal { get; set; }

    public Period Period { get; set; }

    [Display(Name = "Status")]
    public BillPayStatus BillPayStatus { get; set; }
}