using System.ComponentModel.DataAnnotations;
using CustomerApplication.Models;

namespace CustomerApplication.ViewModels;

public class BillPayScheduleViewModel
{
    [Display(Name = "Account")]
    public int? AccountNumber { get; set; }

    public AccountType AccountType { get; set; }

    [UIHint("AccountSelection")]
    public List<AccountViewModel> AccountViewModels { get; set; }

    public decimal? Amount { get; set; }

    [Display(Name = "Scheduled time")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
    public DateTime? ScheduledTimeLocal{ get; set; }

    public Period Period { get; set; }

    [Display(Name = "Payee ID")]
    public int? PayeeID { get; set; }
}