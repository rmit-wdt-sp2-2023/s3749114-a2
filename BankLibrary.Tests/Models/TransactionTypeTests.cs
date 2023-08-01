using BankLibrary.Models;
using Xunit;

namespace BankLibrary.Tests.Models;

public class TransactionTypeTests
{
    [Theory]
    [InlineData(TransactionType.Withdraw)]
    [InlineData(TransactionType.Transfer)]
    [InlineData(TransactionType.BillPay)]
    [InlineData(TransactionType.ServiceCharge)]
    [InlineData(TransactionType.Deposit)]
    public void ServiceCharge_ReturnsDecimal(TransactionType transactionType)
    {
        if (transactionType == TransactionType.Withdraw)
            Assert.Equal(0.05M, transactionType.ServiceCharge());
        else if (transactionType == TransactionType.Transfer)
            Assert.Equal(0.10M, transactionType.ServiceCharge());
        else
            Assert.Equal(0M, transactionType.ServiceCharge());
    }
}