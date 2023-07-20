using CustomerApplication.Models;

namespace CustomerApplication.ViewModels;

public class BillPayViewModel
{
    public int BillPayID { get; set; }

    public int AccountNumber { get; set; }

    public int PayeeID { get; set; }

    public decimal Amount { get; set; }

    public DateTime ScheduledTimeUtc { get; set; } = DateTime.Now;

    public Period Period { get; set; }

    public BillPayStatus BillPayStatus { get; set; } 

    public List<AccountViewModel> AccountViewModels { get; set; }
}