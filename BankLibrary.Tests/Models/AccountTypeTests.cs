using BankLibrary.Models;
using Xunit;

namespace BankLibrary.Tests.Models;

public class AccountTypeTests
{
    [Theory]
    [InlineData(AccountType.Checking)]
    [InlineData(AccountType.Saving)]
    public void MinBalance_NoState_ReturnsDecimal(AccountType accountType)
    {
        if (accountType == AccountType.Checking)
            Assert.Equal(300M, accountType.MinBalance());
        else if (accountType == AccountType.Saving)
            Assert.Equal(0M, accountType.MinBalance());
    }
}