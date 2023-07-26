using CustomerApplication.Models;
using CustomerApplication.ViewModels;
using X.PagedList;

namespace CustomerApplication.Mappers;

public static class ViewModelMapper
{
    public static List<AccountViewModel> Accounts(List<Account> accounts)
    {
        List<AccountViewModel> accountsVM = new();

        foreach (Account a in accounts)
        {
            accountsVM.Add(new AccountViewModel
            {
                AccountNumber = a.AccountNumber,
                AccountType = a.AccountType,
                Balance = a.Balance(),
                AvailableBalance = a.AvailableBalance()
            });
        }
        return accountsVM;
    }

    public static BillPayViewModel BillPay(BillPay billPay)
    {
        return new BillPayViewModel()
        {
            BillPayID = billPay.BillPayID,
            AccountNumber = billPay.AccountNumber,
            PayeeID = billPay.PayeeID,
            Amount = billPay.Amount,
            ScheduledTimeLocal = billPay.ScheduledTimeUtc.ToLocalTime(),
            Period = billPay.Period,
            BillPayStatus = billPay.BillPayStatus
        };
    }

    public static List<BillPayViewModel> BillPays(List<BillPay> billPays)
    {
        List<BillPayViewModel> billPayVM = new();

        foreach (BillPay b in billPays)
            billPayVM.Add(BillPay(b));

        return billPayVM;
    }

    public static CreateTransactionViewModel CreateTransaction(TransactionType transactionType,
        List<Account> accounts, CreateTransactionViewModel createTransactionVM = null)
    {
        List<AccountViewModel> accountVM = Accounts(accounts);

        if (createTransactionVM is null)
            return new CreateTransactionViewModel
            {
                TransactionType = transactionType,
                AccountViewModels = accountVM
            };
        else
        {
            Console.WriteLine("createTransactionVM not null");
            createTransactionVM.AccountViewModels = accountVM;
            return createTransactionVM;
        }
    }

    public static BillPayScheduleViewModel BillPaySchedule(
        List<Account> accounts, BillPayScheduleViewModel billPayScheduleVM = null)
    {
        List<AccountViewModel> accountVM = Accounts(accounts);

        if (billPayScheduleVM is null)
            return new BillPayScheduleViewModel()
            {
                AccountViewModels = accountVM
            };
        else
        {
            billPayScheduleVM.AccountViewModels = accountVM;
            return billPayScheduleVM;
        }
    }

    public static ProfileViewModel Profile(Customer customer)
    {
        return new ProfileViewModel
        {
            Name = customer.Name,
            TFN = customer.TFN,
            Address = customer.Address,
            City = customer.City,
            State = customer.State,
            PostCode = customer.PostCode,
            Mobile = customer.Mobile,
            ProfilePicture = customer.ProfilePicture
        };
    }

    public static StatementsViewModel Statements(List<Account> accounts)
    {
        return new StatementsViewModel
        {
            AccountViewModels = Accounts(accounts)
        };
    }

    public static ReceiptViewModel Receipt(List<Transaction> transactions)
    {
        ReceiptViewModel receiptVM = new();

        foreach (Transaction t in transactions)
        {
            if (t.TransactionType == TransactionType.Deposit || t.TransactionType == TransactionType.Withdraw ||
                t.TransactionType == TransactionType.Transfer && t.DestinationNumber != null)
            {
                receiptVM.TransactionType = t.TransactionType;
                receiptVM.AccountNumber = t.AccountNumber;
                receiptVM.DestinationNumber = t.DestinationNumber;
                receiptVM.Amount = t.Amount;
                receiptVM.Comment = t.Comment;
                receiptVM.TransactionTimeLocal = t.TransactionTimeUtc.ToLocalTime();
            }
            else if (t.TransactionType == TransactionType.ServiceCharge)
                receiptVM.ServiceCharge = t.Amount;
        }
        return receiptVM;
    }
}