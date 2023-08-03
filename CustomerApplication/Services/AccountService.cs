using System.ComponentModel.DataAnnotations;
using BankLibrary.Models;
using BankLibrary.Data;

namespace CustomerApplication.Services
{
	public class AccountService
	{
        private readonly BankContext _context;

        public AccountService(BankContext context) => _context = context;

        public List<Account> GetAccounts(int customerID)
        {
            Customer customer = _context.Customers.Find(customerID);

            if (customer is null)
                return null;

            return customer.Accounts.OrderBy(x => x.AccountNumber).ToList();
        }
            
        public Account GetAccount(int accountNum) => _context.Accounts.Find(accountNum);

        public (ValidationResult, Account) GetAccount(int accountNum, string propertyName)
        {
            Account account = GetAccount(accountNum);

            if (account is null)
                return (new ValidationResult("Account doesn't exist.", new List<string>() { propertyName }), null);

            return (null, account);
        }
    }
}

