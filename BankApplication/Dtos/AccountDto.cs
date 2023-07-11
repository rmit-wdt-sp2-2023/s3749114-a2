namespace BankApplication.Dtos;

// DTO class to represent account data in the JSON. 

public class AccountDto
{
    public int AccountNumber { get; set; }
    public char AccountType { get; set; }
    public int CustomerID { get; set; }
    public List<TransactionDto> Transactions { get; set; }
}