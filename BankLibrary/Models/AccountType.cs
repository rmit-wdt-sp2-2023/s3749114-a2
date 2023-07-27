using System.ComponentModel.DataAnnotations;

namespace BankLibrary.Models;

public enum AccountType
{
    [Display(Name = "Checking")]
    Checking = 1,

    [Display(Name = "Saving")]
    Saving = 2
}

public static class AccountTypeExtensions
{
    public static decimal MinBalance(this AccountType accountType) =>
        accountType == AccountType.Checking ? 300M : 0M;
}