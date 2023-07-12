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