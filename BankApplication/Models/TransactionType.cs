namespace BankApplication.Models;

public enum TransactionType
{
    Deposit = 'D',
    Withdraw = 'W',
    Transfer = 'T',
    ServiceCharge = 'S',
    BillPay = 'B'
}