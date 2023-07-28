using System.ComponentModel.DataAnnotations;

namespace BankLibrary.Models;

public enum LoginStatus
{
    [Display(Name = "Blocked")]
    Blocked = 1,

    [Display(Name = "Unblocked")]
    Unblocked = 2
}

