using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankLibrary.Validation;

namespace BankLibrary.Models;

public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Range(1000, 9999)]
    public required int AccountNumber { get; init; }

    [Required]
    public required AccountType AccountType { get; init; }

    [ForeignKey("Customer")]
    [Range(1000, 9999)]
    public required int CustomerID { get; init; }
    public virtual Customer Customer { get; init; }

    [InverseProperty("Account")]
    public virtual List<Transaction> Transactions { get; init; } = new();

    [InverseProperty("Account")]
    public virtual List<BillPay> BillPays { get; init; } = new();

    // Iterates transactions and caculates the total balance of the account.

    public decimal Balance()
    {
        decimal balance = 0;
        foreach (Transaction t in Transactions)
        {
            if (t.TransactionType == TransactionType.Deposit ||
                t.TransactionType == TransactionType.Transfer && t.DestinationNumber is null)
                balance += t.Amount;
            else
                balance -= t.Amount;
        }
        return balance;
    }

    // Calculates the account's available balance according to the minimum balance requirements. 

    public decimal AvailableBalance()
    {
        decimal balance = Balance();
        decimal minBalance = AccountType.MinBalance();
        return balance >= minBalance ? balance - minBalance : 0;
    }

    // Commits a deposit to the account.

    public (List<ValidationResult>, Transaction) Deposit(decimal amount, string comment) =>
        Credit(TransactionType.Deposit, amount, comment);

    // Commits a withdraw to the account.

    public (List<ValidationResult>, List<Transaction>) Withdraw(decimal amount, string comment) =>
        Debit(TransactionType.Withdraw, amount, comment);

    // Commits an incoming transfer to the account.

    public (List<ValidationResult>, Transaction) TransferTo(decimal amount, string comment) =>
        Credit(TransactionType.Transfer, amount, comment);

    // Commits an outgoing transfer from the account.

    public (List<ValidationResult>, List<Transaction>) TransferFrom(
        int? destinationNum, decimal amount, string comment) =>
            Debit(TransactionType.Transfer, amount, comment, destinationNum);

    // Schedules a BillPay.

    public (List<ValidationResult>, BillPay) BillPaySchedule(
        int payeeID, decimal amount, DateTime scheduledTimeLocal, Period period)
    {
        BillPay billPay = new()
        {
            AccountNumber = AccountNumber,
            PayeeID = payeeID,
            Amount = amount,
            ScheduledTimeUtc = scheduledTimeLocal.ToUniversalTime(),
            Period = period,
            BillPayStatus = BillPayStatus.Scheduled
        };
        if (!ValidationMethods.Validate(billPay, out List<ValidationResult> errors))
            return (errors, null);

        BillPays.Add(billPay);
        return (null, billPay);
    }

    // Commits a BillPay transaction to the account. 

    public (List<ValidationResult>, Transaction) BillPay(decimal amount)
    {
        (List<ValidationResult> errors, List<Transaction> transactions) = Debit(TransactionType.BillPay, amount);
        return (errors, transactions?.First());
    }

    // Creates a credit transaction and the completed transaction is returned.

    private (List<ValidationResult>, Transaction) Credit(
        TransactionType transactionType, decimal amount, string comment)
    {
        Transaction transaction = new()
        {
            TransactionType = transactionType,
            AccountNumber = AccountNumber,
            Amount = amount,
            Comment = comment,
        };

        if (!ValidationMethods.Validate(transaction, out List<ValidationResult> errors))
            return (errors, null);

        Transactions.Add(transaction);
        return (null, transaction);
    }

    // Creates a debit transaction and applies the appropriate service charge.
    // Returns the associated transactions if successful or the errors if unsuccessful. 

    private (List<ValidationResult>, List<Transaction>) Debit(
        TransactionType transactionType, decimal amount, string comment = null, int? destinationNum = null)
    {
        List<Transaction> transactions = new()
        {
            new Transaction()
            {
                TransactionType = transactionType,
                AccountNumber = AccountNumber,
                Amount = amount,
                Comment = comment,
                DestinationNumber = destinationNum
            }
        };
        ValidationMethods.Validate(transactions.First(), out List<ValidationResult> errors);

        if (!MeetsMinBalance(amount, transactionType))
        {
            errors.Add(new ValidationResult(
                $"Invalid amount. {AccountType} accounts must have a min balance of {AccountType.MinBalance():C}.",
                new List<string>() { "Amount" }));
        }
        if (transactionType == TransactionType.Transfer && destinationNum is null)
            errors.Add(new ValidationResult("Enter an account number.",
                new List<string>() { "DestinationNumber" }));

        if (transactionType == TransactionType.Transfer && AccountNumber == destinationNum)
            errors.Add(new ValidationResult("Origin and destination account numbers must be different",
                new List<string>() { "DestinationNumber" }));

        if (errors.Count > 0)
            return (errors, null);

        if (transactionType != TransactionType.BillPay && !IsNextTransactionFree())
        {
            transactions.Add(new Transaction()
            {
                TransactionType = TransactionType.ServiceCharge,
                AccountNumber = AccountNumber,
                Amount = transactionType.ServiceCharge()
            });
        }
        foreach (Transaction t in transactions)
            Transactions.Add(t);

        return (null, transactions);
    }

    // Returns true if the amount for a transaction doesn't 
    // bring the account below the minimum balance requirements. 

    private bool MeetsMinBalance(decimal amount, TransactionType transactionType)
    {
        if (transactionType != TransactionType.BillPay && !IsNextTransactionFree())
            amount += transactionType.ServiceCharge();
        return Balance() - amount >= AccountType.MinBalance();
    }

    // Two free transactions are allowed per account.
    // Deposit and incoming transfer don't consume a free transaction.
    // Returns true if the next transaction is free. If not, returns false. 

    private bool IsNextTransactionFree()
    {
        int count = 0;
        foreach (Transaction t in Transactions)
        {
            if (t.TransactionType == TransactionType.Withdraw ||
                t.TransactionType == TransactionType.Transfer && t.DestinationNumber is not null)
            {
                count++;
                if (count == 2)
                    return false;
            }
        }
        return true;
    }
}