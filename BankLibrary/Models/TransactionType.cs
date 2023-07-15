using System.ComponentModel;

namespace BankLibrary.Models;

public enum TransactionType
{
    [Description("Deposit")]
    Deposit = 1,

    [Description("Withdraw")]
    Withdraw = 2,

    [Description("Transfer")]
    Transfer = 3,

    [Description("Service charge")]
    ServiceCharge = 4,

    [Description("BillPay")]
    BillPay = 5
}

public static class TransactionTypeExtensions
{
    // Defines the service charge associated with each transaction type. 

    public static decimal ServiceCharge(this TransactionType transactionType) => transactionType switch
    {
        TransactionType.Withdraw => 0.05M,
        TransactionType.Transfer => 0.10M,
        _ => 0M
    };
}