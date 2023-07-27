using System.ComponentModel.DataAnnotations;

namespace BankLibrary.Models;

public enum Period
{
    [Display(Name = "One-off")]
    OneOff = 1,

    [Display(Name = "Monthly")]
    Monthly = 2
}