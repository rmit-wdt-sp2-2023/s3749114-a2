using System.ComponentModel.DataAnnotations;
using CustomerApplication.Data;
using CustomerApplication.Models;

namespace CustomerApplication.Services;

public class BillPayService
{
    private readonly BankContext _context;

    private readonly AccountService _accountService;

    public BillPayService(BankContext context, AccountService accountService)
	{
        _context = context;
        _accountService = accountService;
    }

    public BillPay GetBillPay(int billPayID) => _context.BillPays.FirstOrDefault(x => x.BillPayID == billPayID);

    public List<BillPay> GetBillPays(int customerID)
    {
        List<Account> accounts = _accountService.GetAccounts(customerID);

        if (accounts is null)
            return null;

        List<BillPay> billPays = new();

        foreach (Account a in _accountService.GetAccounts(customerID))
            billPays.AddRange(_context.BillPays.Where(x => x.AccountNumber == a.AccountNumber).ToList());

        return billPays.OrderBy(x => x.ScheduledTimeUtc).ToList();
    }

    public ValidationResult CancelBillPay(int billPayID)
    {
        BillPay billPay = GetBillPay(billPayID);

        if (billPay is null)
            return new ValidationResult("Could not find BillPay.", new List<string>() { "BillPayID" });

        _context.BillPays.Remove(billPay);
        _context.SaveChanges();

        return null;
    }

    public List<ValidationResult> SubmitBillPay(
        int accountNum, int payeeID, decimal amount, DateTime ScheduledTimeLocal, Period period)
    {
        (List<ValidationResult> errors, BillPay billPay) =
            BillPay(accountNum, payeeID, amount, ScheduledTimeLocal, period);

        if (errors is null)
        {
            billPay.ScheduledTimeUtc = billPay.ScheduledTimeUtc;
            _context.BillPays.Add(billPay);
            _context.SaveChanges();
        }
        return errors;
    }

    public List<ValidationResult> ConfirmBillPay(
        int accountNum, int payeeID, decimal amount, DateTime scheduledTimeLocal, Period period)
    {
        (List<ValidationResult> errors, _) = BillPay(accountNum, payeeID, amount, scheduledTimeLocal, period);
        return errors;
    }

    private (List<ValidationResult>, BillPay) BillPay(
        int accountNum, int payeeID, decimal amount, DateTime scheduledTimeLocal, Period period)
    {
        List<ValidationResult> errors = new();

        (ValidationResult accountError, Account account) = _accountService.GetAccount(accountNum, "AccountNumber");

        if (accountError is not null)
            errors.Add(accountError);

        (ValidationResult payeeError, _) = GetPayee(payeeID, "PayeeID");

        if (payeeError is not null)
            errors.Add(payeeError);

        return errors.Count > 0
            ? (errors, null)
            : account.BillPaySchedule(payeeID, amount, scheduledTimeLocal, period);
    }

    private (ValidationResult, Payee) GetPayee(int payeeId, string propertyName)
    {
        Payee payee = _context.Payees.Find(payeeId);

        if (payee is null)
            return (new ValidationResult("Payee doesn't exist.", new List<string>() { propertyName }), null);

        return (null, payee);
    }
}