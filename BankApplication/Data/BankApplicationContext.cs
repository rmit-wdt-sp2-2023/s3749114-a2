using Microsoft.EntityFrameworkCore;
using BankApplication.Models;

namespace BankApplication.Data;

public class BankApplicationContext : DbContext
{
    public BankApplicationContext(DbContextOptions<BankApplicationContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Login> Logins { get; set; }
    public DbSet<BillPay> BillPays { get; set; }
    public DbSet<Payee> Payees { get; set; }
}