using CustomerApplication.Data;
using Microsoft.EntityFrameworkCore;
using CustomerApplication.Models;
using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.BackgroundServices;

public class BillPayBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;

    public BillPayBackgroundService(IServiceProvider services) => _services = services;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await PayBills(cancellationToken);
            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
    }

    private async Task PayBills(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _services.CreateScope();
        BankContext context = scope.ServiceProvider.GetRequiredService<BankContext>();
        List<BillPay> billpays = await context.BillPays.ToListAsync(cancellationToken);

        foreach (BillPay b in billpays)
            if (b.ScheduledTimeUtc <= DateTime.UtcNow && b.BillPayStatus == BillPayStatus.Scheduled)
            {
                Account account = context.Accounts.Find(b.AccountNumber);
                if (account is not null)
                {
                    (List<ValidationResult> errors, Transaction transaction) = account.BillPay(b.Amount);
                    if (errors is null)
                    {
                        Console.WriteLine("ERRORS IS NULL");
                        context.Transactions.Add(transaction);
                        if (b.Period == Period.Monthly)
                        {
                            b.ScheduledTimeUtc = b.ScheduledTimeUtc.AddMonths(1);
                            context.BillPays.Update(b);
                        }
                        else
                            context.BillPays.Remove(b);
                    }
                    else
                    {
                        b.BillPayStatus = BillPayStatus.Failed;
                        context.BillPays.Update(b);
                    }
                }
            }
        await context.SaveChangesAsync(cancellationToken);
    }
}