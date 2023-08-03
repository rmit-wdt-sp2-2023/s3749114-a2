using System.ComponentModel.DataAnnotations;
using BankLibrary.Data;
using BankLibrary.Models;
using CustomerApplication.Data;
using CustomerApplication.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CustomerApplication.Tests.Services;

public class CustomerServiceTests : IDisposable
{
    private readonly BankContext _context;

    private readonly CustomerService _customerService;

    public CustomerServiceTests()
	{
        _context = new BankContext(new DbContextOptionsBuilder<BankContext>()
            .UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared").Options);

        _context.Database.EnsureCreated();

        SeedData.Initialise(_context);

        _customerService = new CustomerService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void GetCustomer_CustomerExists_ReturnsCustomer()
    {
        Customer customer = _customerService.GetCustomer(2100);
        Assert.NotNull(customer);
        Assert.Equal("Matthew Bolger", customer.Name);
        Assert.Null(customer.TFN);
        Assert.Equal("123 Fake Street", customer.Address);
        Assert.Equal("Melbourne", customer.City);
        Assert.Null(customer.State);
        Assert.Equal("3000", customer.PostCode);
        Assert.Null(customer.Mobile);
        Assert.Null(customer.ProfilePicture);
    }

    [Fact]
    public void GetCustomer_CustomerDoesntExists_ReturnsNull()
    {
        Customer customer = _customerService.GetCustomer(5000);
        Assert.Null(customer);
    }

    [Theory]
    [InlineData(2200, "John Smith", "123 123 123", "88 Test Street", "Brisbane", "QLD", "5000", "0412 654 876")]
    [InlineData(2200, "Rodney Cocker", null, null, null, null, null, null)]
    public void UpdateCustomer_ValidParameters_UpdatesDatabase(int customerID, string name,
        string tfn, string address, string city, string state, string postCode, string mobile)
    {
        List<ValidationResult> errors = _customerService.UpdateCustomer(
            customerID, name, tfn, address, city, state, postCode, mobile);

        Customer customer = _customerService.GetCustomer(customerID);

        Assert.Null(errors);
        Assert.Equal(name, customer.Name);
        Assert.Equal(tfn, customer.TFN);
        Assert.Equal(address, customer.Address);
        Assert.Equal(city, customer.City);
        Assert.Equal(state, customer.State);
        Assert.Equal(postCode, customer.PostCode);
        Assert.Equal(mobile, customer.Mobile);
    }

    [Theory]
    [InlineData(2200, "Bob", "234567899", "89 Test Street", "Sydney", "NSW", "6000", "0412 654 874")]
    [InlineData(2200, "John Smith", "123123123", "89 Test Street", "Sydney", "NSW", "6000", "0412 654 874")]
    [InlineData(2200, "John Smith", "234 567 899", "89 Test Street", "Sydney", "Queensland", "3070", "0412 654 476")]
    [InlineData(2200, "John Smith", "234 567 899", "89 Test Street", "Sydney", "NSW", "60707", "0412 654 874")]
    [InlineData(2200, "John Smith", "234 567 899", "89 Test Street", "Sydney", "NSW", "6070", "0412654876")]
    [InlineData(2200, "John Smith", "234 567 899", "89 Test Street", "Sydney", "NSW", "6070", "Incorrect Number")]
    [InlineData(2200, "This is more than fifty characters in length, which is the limit", "234 567 899", "89 Test Street", "Sydney", "tas", "6070", "0412 654 874")]
    [InlineData(2200, "John Smith", "234 567 899", "89 Test Street", "This is more than fifty characters in length, which is the limit", "tas", "6070", "0412 654 874")]
    public void UpdateCustomer_InvalidParameters_ReturnsErrors(int customerID, string name,
        string tfn, string address, string city, string state, string postCode, string mobile)
    {
        List<ValidationResult> errors = _customerService.UpdateCustomer(
            customerID, name, tfn, address, city, state, postCode, mobile);

        Customer customer = _customerService.GetCustomer(customerID);

        Assert.NotNull(errors);
        Assert.NotEqual(name, customer.Name);
        Assert.NotEqual(tfn, customer.TFN);
        Assert.NotEqual(address, customer.Address);
        Assert.NotEqual(city, customer.City);
        Assert.NotEqual(state, customer.State);
        Assert.NotEqual(postCode, customer.PostCode);
        Assert.NotEqual(mobile, customer.Mobile);
    }

    // TODO test photos - TBC storing image in database as link or binary
}

