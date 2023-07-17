using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CustomerApplication.Models;

public enum TransactionType
{
    Deposit = 1,

    Withdraw = 2,

    Transfer = 3,

    [Display(Name = "Service Charge")]
    ServiceCharge = 4,

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