using System.ComponentModel.DataAnnotations;

namespace BankLibrary.Models;

public enum LoginStatus
{
    [Display(Name = "Locked")]
    Locked = 1,

    [Display(Name = "Unlocked")]
    Unlocked = 2
}

