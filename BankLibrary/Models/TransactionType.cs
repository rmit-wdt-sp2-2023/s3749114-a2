using System.ComponentModel.DataAnnotations;

namespace BankLibrary.Models;

public enum TransactionType
{
    [Display(Name = "Deposit")]
    Deposit = 1,

    [Display(Name = "Withdraw")]
    Withdraw = 2,

    [Display(Name = "Transfer")]
    Transfer = 3,

    [Display(Name = "Service charge")]
    ServiceCharge = 4,

    [Display(Name = "BillPay")]
    BillPay = 5
}

public static class TransactionTypeExtensions
{
    public static decimal ServiceCharge(this TransactionType transactionType)
    {
        if (transactionType == TransactionType.Withdraw)
            return 0.05M;
        else if (transactionType == TransactionType.Transfer)
            return 0.10M;
        else
            return 0M;
    }
}