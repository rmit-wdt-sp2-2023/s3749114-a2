using System.ComponentModel.DataAnnotations;

namespace BankLibrary.Models;

public enum BillPayStatus
{
    [Display(Name = "Blocked")]
    Blocked = 1,

    [Display(Name = "Failed")]
    Failed = 2,

    [Display(Name = "Scheduled")]
    Scheduled = 3
}