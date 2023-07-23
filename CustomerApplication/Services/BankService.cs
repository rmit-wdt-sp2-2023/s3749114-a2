using SimpleHashing.Net;
using CustomerApplication.Data;
using CustomerApplication.Models;
using CustomerApplication.Validation;
using System.ComponentModel.DataAnnotations;
using CustomerApplication.Utilities;
using static System.Net.Mime.MediaTypeNames;
using ImageMagick;
using System.IO;
using System.Drawing;

namespace CustomerApplication.Services;

public class BankService
{
    private readonly BankContext _context;

    private readonly string _directory;

    private static readonly ISimpleHash SimpleHash = new SimpleHash();

    public BankService(BankContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _directory = Path.Combine(webHostEnvironment.WebRootPath, "ProfilePictures");
    }

    public Login Login(string loginID, string password)
    {
        Login login = _context.Logins.Find(loginID);
        if (login is not null)
        {
            if (SimpleHash.Verify(password, login.PasswordHash))
                return login;
        }
        return null;
    }

    public List<Account> GetAccounts(int customerID) =>
        _context.Customers.Find(customerID).Accounts.OrderBy(x => x.AccountNumber).ToList();

    public Account GetAccount(int accountNum) => _context.Accounts.Find(accountNum);



    public List<Transaction> GetTransactions(int accountNum)
    {
        Account account = _context.Accounts.Find(accountNum);
        if (account is not null)
        {
            List<Transaction> transactions = account.Transactions.OrderByDescending(x => x.TransactionTimeUtc).ToList();
            if (transactions.Count > 0)
                return transactions;
        }
        return null;
    }

    private (ValidationResult, Account) GetAccount(int accountNum, string propertyName)
    {
        Account account = _context.Accounts.Find(accountNum);
        if (account is null)
            return (new ValidationResult("Account doesn't exist.", new List<string>() { propertyName }), null);
        return (null, account);
    }

    private (ValidationResult, Payee) GetPayee(int payeeId, string propertyName)
    {
        Payee payee = _context.Payees.Find(payeeId);
        if (payee is null)
            return (new ValidationResult("Payee doesn't exist.", new List<string>() { propertyName }), null);
        return (null, payee);
    }

    // Methods validate transactions but don't update database.

    public List<ValidationResult> ConfirmDeposit(int accountNum, decimal amount, string comment)
    {
        (List<ValidationResult> errors, _) = Deposit(accountNum, amount, comment);
        return errors;
    }

    public List<ValidationResult> ConfirmWithdraw(int accountNum, decimal amount, string comment)
    {
        (List<ValidationResult> errors, _) = Withdraw(accountNum, amount, comment);
        return errors;
    }

    public List<ValidationResult> ConfirmTransfer(int accountNum, int? destinationNum, decimal amount, string comment)
    {
        (List<ValidationResult> errors, _) = Transfer(accountNum, destinationNum, amount, comment);
        return errors;
    }

    public List<ValidationResult> ConfirmBillPay(
        int accountNum, int payeeID, decimal amount, DateTime ScheduledTimeUtc, Period period)
    {
        (List<ValidationResult> errors, _) = BillPay(accountNum, payeeID, amount, ScheduledTimeUtc, period);
        return errors;
    }

    private (List<ValidationResult>, BillPay) BillPay(int accountNum, int payeeID, decimal amount, DateTime ScheduledTimeUtc, Period period)
    {
        List<ValidationResult> errors = new();

        (ValidationResult accountError, Account account) = GetAccount(accountNum, "AccountNumber");

        if (accountError is not null)
            errors.Add(accountError);

        (ValidationResult payeeError, _) = GetPayee(payeeID, "PayeeID");

        if (payeeError is not null)
            errors.Add(payeeError);

        return errors.Count > 0 ? (errors, null) : account.BillPaySchedule(payeeID, amount, ScheduledTimeUtc, period);
    }

    // Methods validate transactions and update the database if valid.

    public List<ValidationResult> SubmitBillPay(int accountNum, int payeeID, decimal amount, DateTime ScheduledTimeUtc, Period period)
    {
        (List<ValidationResult> errors, BillPay billPay) = BillPay(accountNum, payeeID, amount, ScheduledTimeUtc, period);
        if (errors is null)
        {
            billPay.ScheduledTimeUtc = billPay.ScheduledTimeUtc.ToUniversalTime();
            _context.BillPays.Add(billPay);
            _context.SaveChanges();
        }
        return errors;
    }

    public List<ValidationResult> SubmitDeposit(int accountNum, decimal amount, string comment)
    {
        (List<ValidationResult> errors, Transaction transaction) = Deposit(accountNum, amount, comment);
        if (errors is null)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }
        return errors;
    }

    public List<ValidationResult> SubmitWithdraw(int accountNum, decimal amount, string comment)
    {
        (List<ValidationResult> errors, List<Transaction> transactions) = Withdraw(accountNum, amount, comment);
        if (errors is null)
        {
            foreach (Transaction t in transactions)
                _context.Transactions.Add(t);
            _context.SaveChanges();
        }
        return errors;
    }

    public List<ValidationResult> SubmitTransfer(int accountNum, int? destinationNum, decimal amount, string comment)
    {
        (List<ValidationResult> errors, List<Transaction> transactions) =
            Transfer(accountNum, destinationNum, amount, comment);

        if (errors is null)
        {
            foreach (Transaction t in transactions)
                _context.Transactions.Add(t);
            _context.SaveChanges();
        }
        return errors;
    }

    // Methods to help validate transactions but don't update the database.
    // Returns all associated transaction if valid or errors if invalid.

    private (List<ValidationResult>, Transaction) Deposit(int accountNum, decimal amount, string comment)
    {
        (ValidationResult error, Account account) = GetAccount(accountNum, "AccountNumber");
        return error is not null ? (new List<ValidationResult>() { error }, null) : account.Deposit(amount, comment);
    }

    private (List<ValidationResult>, List<Transaction>) Withdraw(int accountNum, decimal amount, string comment)
    {
        (ValidationResult error, Account account) = GetAccount(accountNum, "AccountNumber");
        return error is not null ? (new List<ValidationResult>() { error }, null) : account.Withdraw(amount, comment);
    }

    private (List<ValidationResult>, List<Transaction>) Transfer(
        int accountNum, int? destinationNum, decimal amount, string comment)
    {
        List<ValidationResult> errors = new();
        List<Transaction> transactions = new();

        TransferFrom(accountNum, destinationNum, amount, comment, ref errors, ref transactions);
        TransferTo(destinationNum, amount, comment, ref errors, ref transactions);

        return errors.Count > 0 ? (errors, null) : (null, transactions);
    }

    // Methods to help validate transfer transactions but don't update the database.

    private void TransferTo(int? destinationNum, decimal amount, string comment,
        ref List<ValidationResult> errors, ref List<Transaction> transactions)
    {
        if (destinationNum is null)
            errors.Add(new ValidationResult("Enter an account number.", new List<string>() { "DestinationNumber" }));
        else
        {
            (ValidationResult error, Account destinationAccount) = GetAccount(
                destinationNum.GetValueOrDefault(), "DestinationNumber");

            if (error is not null)
                errors.Add(error);
            else
            {
                (_, Transaction transaction) = destinationAccount.TransferTo(amount, comment);
                transactions.Add(transaction);
            }
        }
    }

    private void TransferFrom(int accountNum, int? destinationNum, decimal amount,
        string comment, ref List<ValidationResult> errors, ref List<Transaction> transactions)
    {
        (ValidationResult error, Account account) = GetAccount(accountNum, "AccountNumber");

        if (error is not null)
            errors.Add(error);
        else
        {
            (List<ValidationResult> errorsFrom, List<Transaction> transactionsFrom) =
                account.TransferFrom(destinationNum, amount, comment);

            if (errorsFrom is not null)
                errors.AddRange(errorsFrom);
            else
                transactions.AddRange(transactionsFrom);
        }
    }

    /* * * * * * * * * * * * * * * * * 
     *     Customers / Profiles      *
     * * * * * * * * * * * * * * * * */

    public Customer GetCustomer(int customerID) => _context.Customers.FirstOrDefault(c => c.CustomerID == customerID);

    public List<ValidationResult> UpdateCustomer(int customerID, string name, string TFN,
        string address, string city, string state, string postCode, string mobile)
    {
        Customer customer = GetCustomer(customerID);
        List<ValidationResult> errors = new();

        if (customer is null)
        {
            errors.Add(new ValidationResult("Could not update. Can't find customer.", new List<string>() { "Other" }));
            return errors;
        }
        customer.Name = name;
        customer.TFN = TFN;
        customer.Address = address;
        customer.City = city;
        customer.State = state?.ToUpper();
        customer.PostCode = postCode;
        customer.Mobile = mobile;

        if (!ValidationMethods.Validate(customer, out errors))
            return errors;

        _context.Customers.Update(customer);
        _context.SaveChanges();

        return null;
    }

    public List<ValidationResult> ChangePassword(int customerID, string oldPass, string newPass, string confirmPass) 
    {
        List<ValidationResult> errors = new();

        if (oldPass is null)
            errors.Add(new ValidationResult("Enter old password.", new List<string>() { "OldPassword" }));
        if (newPass is null)
            errors.Add(new ValidationResult("Enter new password.", new List<string>() { "NewPassword" }));
        if (newPass != confirmPass)
            errors.Add(new ValidationResult("Passwords don't match.", new List<string>() { "ConfirmPassword" }));

        if (errors.Count > 0)
            return errors;

        Login login = _context.Logins.FirstOrDefault(c => c.CustomerID == customerID);

        if (login is null)
            errors.Add(new ValidationResult("Error, couldn't find customer.", new List<string>() { "PasswordFailed" }));
        else
            if (!SimpleHash.Verify(oldPass, login.PasswordHash))
                errors.Add(new ValidationResult("Incorrect password.", new List<string>() { "OldPassword" }));

        if (errors.Count > 0)
            return errors;

        login.PasswordHash = SimpleHash.Compute(newPass);

        _context.Logins.Update(login);
        _context.SaveChanges();

        return null;
    }

    public (ValidationResult, string fileName) UploadProfilePicture(int customerID, IFormFile profileImage)
    {
        if (profileImage is null)
            return (new ValidationResult("You must select an image.", new List<string>() { "ProfileImage" }), null);

        Customer customer = GetCustomer(customerID);

        if (customer is null)
            return (new ValidationResult(
                "Update unsuccessful. Unable to find customer.", new List<string>() { "ProfileImage" }), null);

        // ----

        string ext = Path.GetExtension(profileImage.FileName).ToLowerInvariant();

        string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".heic" };

        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            return (new ValidationResult("Invalid file type.", new List<string>() { "ProfileImage" }), null);

        string tempFilePath = Path.Combine(_directory, $"{customerID}-temp-{ext}");

        using FileStream fileStream = new(tempFilePath, FileMode.Create);

        profileImage.CopyTo(fileStream);

        // -----

        if (Directory.Exists(tempFilePath) || File.Exists(tempFilePath))
        {
            Console.WriteLine("IT EXISTS");
        }

        string newFileName = $"{customerID}.jpg";
        string newFilePath = Path.Combine(_directory, newFileName);

        try
        {

      
        using MagickImage image = new(tempFilePath);

 
        image.Resize(new MagickGeometry(400, 400));
        image.Format = MagickFormat.Jpg;
        image.Quality = 100;
        image.Alpha(AlphaOption.Remove);
        image.BackgroundColor = new MagickColor("#FFFFFF");



        image.Write(newFilePath);

        }
        catch(MagickCoderErrorException)
        {
            return (new ValidationResult("Upload failed. Image may be corrupt. Try a different image",
                new List<string>() { "ProfileImage" }), null);
        }



        File.Delete(tempFilePath);

        //-----

        customer.ProfilePicture = newFileName;

        _context.Customers.Update(customer);
        _context.SaveChanges();

        return (null, newFileName);
    }

    public List<ValidationResult> RemoveProfilePicture(int customerID)
    {
        List<ValidationResult> errors = new();
        Customer customer = GetCustomer(customerID);

        if (customer is null)
        {
            errors.Add(new ValidationResult("Could not update. Can't find customer.", new List<string>() { "Other" }));
            return errors;
        }

        if (customer.ProfilePicture is null)
        {
            errors.Add(new ValidationResult("No profile picture to remove.", new List<string>() { "ProfilePicture" }));
            return errors;
        }

        string filePath = Path.Combine(_directory, customer.ProfilePicture);

        try
        {
            File.Delete(filePath);
        }
        catch (Exception)
        {
            errors.Add(new ValidationResult(
                "Couldn't remove profile picture.", new List<string>() { "ProfilePicture" }));
        }
        customer.ProfilePicture = null;

        _context.Customers.Update(customer);
        _context.SaveChanges();

        return null;
    }

    /* * * * * * * * * * * * * * * * * 
     *            BillPays           *
     * * * * * * * * * * * * * * * * */

    public List<BillPay> GetBillPays(int customerID)
    {
        List<BillPay> billPays = new();

        foreach (Account a in GetAccounts(customerID))
            billPays.AddRange(_context.BillPays.Where(x => x.AccountNumber == a.AccountNumber).ToList());

        return billPays.OrderBy(x => x.ScheduledTimeUtc).ToList();
    }

    public BillPay GetBillPay(int billPayID) => _context.BillPays.FirstOrDefault(x => x.BillPayID == billPayID);

    public ValidationResult CancelBillPay(int billPayID)
    {

        BillPay billPay = _context.BillPays.FirstOrDefault(x => x.BillPayID == billPayID);

        if (billPay is not null)
        {
            _context.BillPays.Remove(billPay);
            _context.SaveChanges();
            return null;
        }
        return new ValidationResult("Could not find BillPay.", new List<string>() { "BillPayID" });
    }
}