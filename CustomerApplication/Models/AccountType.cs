namespace CustomerApplication.Models;

public enum AccountType
{
    Checking = 1,
    Saving = 2
}

public static class AccountTypeExtensions
{
    public static decimal MinBalance(this AccountType accountType) =>
        accountType == AccountType.Checking ? 300M : 0M;
}