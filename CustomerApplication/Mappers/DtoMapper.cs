using BankLibrary.Models;
using CustomerApplication.Dtos;

namespace CustomerApplication.Mappers;

// This class is used to map DTOs from the web service into business objects.
// There are mapper functions for Customer, Account, Transaction and Login. 

public static class DtoMapper
{
    public static List<Customer> ConvertCustomersFromDto(List<CustomerDto> customersDto) 
    {
        List<Customer> customers = new();
        foreach(CustomerDto customerDto in customersDto) 
            customers.Add(ConvertCustomerFromDto(customerDto));
        return customers;
    }

    public static Customer ConvertCustomerFromDto(CustomerDto customerDto)
    {
        List<Account> accounts = new();
        foreach(AccountDto accountDto in customerDto.Accounts) 
            accounts.Add(ConvertAccountFromDto(accountDto));
        return new Customer() 
        { 
            CustomerID = customerDto.CustomerID,
            Name = customerDto.Name,
            Address = customerDto.Address,
            City = customerDto.City,
            PostCode = customerDto.PostCode,
            Accounts = accounts                                               
        };
    }

    // Calling the appropriate Account constructor ensures
    // the balance is set to the sum of associated transactions.

    public static Account ConvertAccountFromDto(AccountDto accountDto)
    {
        List<Transaction> transactions = new();
        foreach(TransactionDto transactionDto in accountDto.Transactions) 
            transactions.Add(ConvertTransactionFromDto(transactionDto, accountDto.AccountNumber));
        return new Account()
        {
            AccountNumber = accountDto.AccountNumber,
            AccountType = accountDto.AccountType == 'C' ? AccountType.Checking : AccountType.Saving,
            CustomerID = accountDto.CustomerID,
            Transactions = transactions
        };
    }

    // When inserting a Transaction from the web
    // service, TransactionType.D (deposit) is set. 

    public static Transaction ConvertTransactionFromDto(TransactionDto transactionDto, int accountNumber)
    {
        return new Transaction()
        {
            TransactionType = TransactionType.Deposit,
            AccountNumber = accountNumber,
            DestinationNumber = null,
            Amount = transactionDto.Amount,
            Comment = transactionDto.Comment,
            TransactionTimeUtc = transactionDto.TransactionTimeUtc
        };
    }

    public static List<Login> ConvertLoginsFromDto(List<CustomerDto> customersDto)
    {
        List<Login> logins = new();
        foreach(CustomerDto customer in customersDto) 
            logins.Add(ConvertLoginFromDto(customer.Login, customer.CustomerID));
        return logins;
    }

    public static Login ConvertLoginFromDto(LoginDto loginDto, int customerID)
    {
        return new Login() 
        { 
            LoginID = loginDto.LoginID,
            CustomerID = customerID,
            PasswordHash = loginDto.PasswordHash                                        
        };
    }
}