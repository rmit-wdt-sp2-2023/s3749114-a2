namespace BankLibrary.Dtos;

// DTO class to represent transaction data in the JSON.

public class TransactionDto
{
    public decimal Amount { get; set; }
    public string Comment { get; set; }
    public DateTime TransactionTimeUtc { get; set; }
}