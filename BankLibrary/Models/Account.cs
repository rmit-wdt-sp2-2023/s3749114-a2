using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankLibrary.Models;

public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account number")]
    [Range(1000, 9999)]
    public int AccountNumber { get; set; }

    [Required]
    [Display(Name = "Account type")]
    public AccountType AccountType { get; set; }

    [ForeignKey("Customer")]
    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }

    [InverseProperty("Account")]
    public virtual List<Transaction> Transactions { get; set; }

    public decimal CalculateBalance()
    {
        decimal balance = 0;
        foreach (Transaction transaction in Transactions)
        {
            balance = transaction.TransactionType switch
            {
                TransactionType.Deposit => balance + transaction.Amount,
                TransactionType.Withdraw => balance - transaction.Amount,
                TransactionType.ServiceCharge => balance - transaction.Amount,
                TransactionType.Transfer when transaction.DestinationNumber is null => balance + transaction.Amount,
                TransactionType.Transfer when transaction.DestinationNumber is not null => balance - transaction.Amount,
                TransactionType.BillPay => balance - transaction.Amount,
                _ => balance
            };
        }
        return balance;
    }

    public Transaction Deposit(decimal amount, string comment) =>
        Credit(TransactionType.Deposit, amount, comment);

    private Transaction Credit(TransactionType transactionType, decimal amount, string comment)
    {
        Transaction transaction = new()
        {
            TransactionType = transactionType,
            AccountNumber = AccountNumber,
            Amount = amount,
            Comment = comment,
        };
        Transactions.Add(transaction);
        return transaction;
    }
}