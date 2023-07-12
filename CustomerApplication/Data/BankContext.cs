using Microsoft.EntityFrameworkCore;
using BankLibrary.Models;

namespace CustomerApplication.Data;

public class BankContext : DbContext
{
    public BankContext(DbContextOptions<BankContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Login> Logins { get; set; }
    public DbSet<BillPay> BillPays { get; set; }
    public DbSet<Payee> Payees { get; set; }

    // Fluent-API.
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Transaction>().ToTable(b => b.HasCheckConstraint("CH_Transaction_Amount", "Amount > 0"));
        builder.Entity<BillPay>().ToTable(b => b.HasCheckConstraint("CH_BillPay_Amount", "Amount > 0"));

    }
}