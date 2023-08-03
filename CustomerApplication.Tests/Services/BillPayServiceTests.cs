using BankLibrary.Data;
using BankLibrary.Models;
using CustomerApplication.Data;
using CustomerApplication.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CustomerApplication.Tests.Services;

public class BillPayServiceTests : IDisposable
{
    private readonly BankContext _context;

    private readonly BillPayService _billPayService;

    public BillPayServiceTests()
	{
        _context = new BankContext(new DbContextOptionsBuilder<BankContext>()
            .UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared").Options);

        _context.Database.EnsureCreated();

        SeedData.Initialise(_context);

        _context.BillPays.Add(new BillPay()
        {
            AccountNumber = 4100,
            PayeeID = 1,
            Amount = 5,
            ScheduledTimeUtc = DateTime.UtcNow.AddMinutes(11),
            Period = Period.OneOff,
            BillPayStatus = BillPayStatus.Scheduled
        });

        _context.SaveChanges();

        AccountService accountService = new(_context);

        _billPayService = new BillPayService(_context, accountService);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void GetBillPay_BillPayExists_ReturnsBillPay()
    {
        int billPayID = 1;

        BillPay billPay = _billPayService.GetBillPay(billPayID);

        Assert.NotNull(billPay);
        Assert.Equal(4100, billPay.AccountNumber);
        Assert.Equal(5, billPay.Amount);
    }

    [Fact]
    public void GetBillPay_BillPayDoesntExists_ReturnsNull()
    {
        int billPayID = 10;

        BillPay billPay = _billPayService.GetBillPay(billPayID);

        Assert.Null(billPay);
    }

    [Fact]
    public void GetBillPays_CustomerExists_ReturnsBillPays()
    {
        int customerID = 2100;

        List<BillPay> billPays = _billPayService.GetBillPays(customerID);

        Assert.NotNull(billPays);
        Assert.True(billPays.Count > 0);
    }

    [Fact]
    public void GetBillPays_CustomerDoesntExists_ReturnsNull()
    {
        int customerID = 5000;

        List<BillPay> billPays = _billPayService.GetBillPays(customerID);

        Assert.Null(billPays);
    }
}