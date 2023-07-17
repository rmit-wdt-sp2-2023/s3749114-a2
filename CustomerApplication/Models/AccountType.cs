namespace CustomerApplication.Models;

public enum AccountType
{
    Checking = 1,
    Saving = 2
}

public static class AccountTypeExtensions
{
    // Defines the minimum balance allowed in each account type.

    public static decimal MinBalance(this AccountType accountType) => accountType switch
    {
        AccountType.Checking => 300M,
        AccountType.Saving => 0M,
        _ => 0M
    };
}