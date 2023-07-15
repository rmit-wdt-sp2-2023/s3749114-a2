using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankLibrary.Models;

public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account Number")]
    [Range(1000, 9999)]
    public int AccountNumber { get; set; }

    [Required]
    [Display(Name = "Account Type")]
    public AccountType AccountType { get; set; }

    [ForeignKey("Customer")]
    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }

    [InverseProperty("Account")]
    public virtual List<Transaction> Transactions { get; set; }

    // Goes through all transactions and caculates the balance.

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

    // Caclulates the account's available balance
    // according to the minimum balance requirements. 

    public decimal AvailableBalance()
    {
        decimal balance = CalculateBalance();
        decimal minBalance = AccountType.MinBalance();
        return balance >= minBalance ? balance - minBalance : 0;
    }

    // Returns true if the balance - (service charge + amount) is more than 
    // or equal to the minimum balance requirements. If not, returns false. 

    private bool MeetsMinBalance(decimal amount, TransactionType transactionType)
    {
        decimal total = amount;
        if (!IsNextTransactionFree())
            total += transactionType.ServiceCharge();
        return CalculateBalance() - total >= AccountType.MinBalance();
    }

    // Two free transactions are allowed per account.
    // Deposit and incoming transfer don't consume a free transaction.
    // Returns true if the next transaction is free. If not, returns false. 

    private bool IsNextTransactionFree()
    {
        int count = 0;
        foreach (Transaction transaction in Transactions)
        {
            if (transaction.TransactionType == TransactionType.Withdraw ||
                transaction.TransactionType == TransactionType.Transfer &&
                transaction.DestinationNumber is not null)
            {
                count++;
                if (count == 2)
                    return false;
            }
        }
        return true;
    }

    // Commits a deposit to the account.
    // Returns the transaction associated with the deposit.

    public Transaction Deposit(decimal amount, string comment) =>
        Credit(TransactionType.Deposit, amount, comment);

    // Commits a withdraw to the account.
    // Returns the transactions associated with the withdraw.
    // If the withdraw failed, an error message will be returned.

    public (List<Transaction> transactions, string message) Withdraw(decimal amount, string comment)
    {
        if (!MeetsMinBalance(amount, TransactionType.Withdraw))
            return (null, $"You must have a min balance of {AccountType.MinBalance():C}.");

        List<Transaction> newTransactions = new();

        if (!IsNextTransactionFree())
            newTransactions.Add(
                Debit(TransactionType.ServiceCharge, TransactionType.Withdraw.ServiceCharge(), null, null));

        newTransactions.Add(Debit(TransactionType.Withdraw, amount, comment, null));

        return (newTransactions, null);
    }

    // Commits a transfer to (incoming) the account.
    // Returns the transaction associated with the transfer.

    public Transaction TransferTo(decimal amount, string comment) =>
        Credit(TransactionType.Transfer, amount, comment);

    // Commits a transfer from (outgoing) the account.
    // If successful, returns the transactions associated with the transfer.
    // If unsuccessful, an error message will be returned. 

    public (List<Transaction> transactions, string message) TransferFrom(
        int? destinationNumber, decimal amount, string comment)
    {
        if (destinationNumber == null)
            return (null, "You must enter an Account Number.");

        if (AccountNumber == destinationNumber)
            return (null, "To and From Account Numbers cannot be the same.");

        if (!MeetsMinBalance(amount, TransactionType.Transfer))
            return (null, $"You must have a minimum balance of {AccountType.MinBalance():C}.");

        List<Transaction> newTransactions = new();

        if (!IsNextTransactionFree())
            newTransactions.Add(Debit(
                TransactionType.ServiceCharge, TransactionType.Transfer.ServiceCharge(), null, null));

        newTransactions.Add(Debit(TransactionType.Transfer, amount, comment, destinationNumber));

        return (newTransactions, null);
    }

    // Creates a credit transaction and adds it to the
    // balance. The completed transaction is returned.

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

    // Creates a debit transaction and subtracts it from
    // the balance. The completed transaction is returned. 

    private Transaction Debit(TransactionType transactionType, decimal amount, string comment, int? destinationNumber)
    {
        Transaction transaction = new()
        {
            TransactionType = transactionType,
            AccountNumber = AccountNumber,
            Amount = amount,
            Comment = comment,
            DestinationNumber = destinationNumber
        };
        Transactions.Add(transaction);
        return transaction;
    }
}
