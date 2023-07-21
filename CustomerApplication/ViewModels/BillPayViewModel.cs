using System.ComponentModel.DataAnnotations;
using CustomerApplication.Models;

namespace CustomerApplication.ViewModels;

public class BillPayViewModel
{
    public int? BillPayID { get; set; }

    [Display(Name = "Account")]
    public int? AccountNumber { get; set; }

    public AccountType AccountType { get; set; }

    public decimal? Amount { get; set; }

    [Display(Name = "Scheduled time")]
    [DisplayFormat(DataFormatString = "{0:dd:mm:yyyhh:mm TT}", ApplyFormatInEditMode = true)]
    public DateTime? ScheduledTimeUtc { get; set; }

    public Period Period { get; set; }

    [Display(Name = "Status")]
    public BillPayStatus BillPayStatus { get; set; } 

    public List<AccountViewModel> AccountViewModels { get; set; }

    [Display(Name = "Payee ID")]
    public int? PayeeID { get; set; }
}