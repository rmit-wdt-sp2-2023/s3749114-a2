using System.ComponentModel;

namespace BankLibrary.Models;

public enum AccountType
{
    [Description("Checking")]
    Checking = 1,

    [Description("Saving")]
    Saving = 2
}