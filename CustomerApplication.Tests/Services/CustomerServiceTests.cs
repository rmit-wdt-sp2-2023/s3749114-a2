using System.ComponentModel.DataAnnotations;
using System.Text;
using BankLibrary.Data;
using BankLibrary.Models;
using CustomerApplication.Data;
using CustomerApplication.Services;
using ImageMagick;
using Microsoft.AspNetCore.Http;
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

        _context.ProfilePictures.Add(new ProfilePicture()
        {
            CustomerID = 2100,
            Image = Encoding.UTF8.GetBytes("dummy image"),
            FileName = $"{2100}-profile-image",
            ContentType = "image/jpg"
        });

        _context.ProfilePictures.Add(new ProfilePicture()
        {
            CustomerID = 2300,
            Image = Encoding.UTF8.GetBytes("dummy image"),
            FileName = $"{2300}-profile-image",
            ContentType = "image/jpg"
        });

        _context.SaveChanges();

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
        Assert.NotNull(customer.ProfilePicture);
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

    [Fact]
    public void GetProfilePicture_NoPicture_ReturnsDefaultPicture()
    {
        int customerID = 2200;

        ProfilePicture profilePicture = _customerService.GetProfilePicture(customerID);

        Assert.NotNull(profilePicture);
        Assert.Equal("default.jpg", profilePicture.FileName);
    }

    [Fact]
    public void GetProfilePicture_HasPicture_ReturnsPicture()
    {
        int customerID = 2100;
        string fileName = $"{customerID}-profile-image";
        byte[] imageData = Encoding.UTF8.GetBytes("dummy image");
        string contentType = "image/jpg";

        ProfilePicture profilePicture = _customerService.GetProfilePicture(customerID);

        Assert.NotNull(profilePicture);
        Assert.Equal(fileName, profilePicture.FileName);
        Assert.Equal(customerID, profilePicture.CustomerID);
        Assert.Equal(imageData, profilePicture.Image);
        Assert.Equal(contentType, profilePicture.ContentType);
    }

    // Cannot mock heic files with Magick.
    // Apparently Magick.NET only supports decoding of HEIC due to licensing issues.

    [Theory]
    [InlineData(2200, MagickFormat.Png, "png")]
    [InlineData(2200, MagickFormat.Jpg, "jpg")]
    [InlineData(2200, MagickFormat.Jpeg, "jpeg")]
    public void UploadProfilePicture_ValidParameters_UpdatesDatabase(
        int customerID, MagickFormat format, string extension)
    {
        IFormFile file = MockIFormFile(format, extension);

        ValidationResult error = _customerService.UploadProfilePicture(customerID, file);

        ProfilePicture profilePicture = _customerService.GetProfilePicture(customerID);

        Assert.Null(error);
        Assert.Equal($"2200-profile-picture.jpg", profilePicture.FileName);
    }

    [Theory]
    [InlineData(2200, MagickFormat.Pdf, "pdf")]
    [InlineData(2200, MagickFormat.Tif, "tif")]
    [InlineData(2200, MagickFormat.Gif, "gif")]
    [InlineData(500000, MagickFormat.Jpg, "jpg")]
    public void UploadProfilePicture_InvalidParameters_ReturnsErrors(
        int customerID, MagickFormat format, string extension)
    {
        IFormFile file = MockIFormFile(format, extension);

        ValidationResult error = _customerService.UploadProfilePicture(customerID, file);

        ProfilePicture profilePicture = _context.ProfilePictures.FirstOrDefault(x => x.CustomerID == customerID);

        Assert.NotNull(error);
        Assert.Null(profilePicture);
    }

    [Fact]
    public void RemoveProfilePicture_HasPicture_UpdateDatabase()
    {
        int customerID = 2300;

        List<ValidationResult> errors = _customerService.RemoveProfilePicture(customerID);

        ProfilePicture profilePicture = _context.ProfilePictures.FirstOrDefault(x => x.CustomerID == customerID);

        Assert.Null(errors);
        Assert.Null(profilePicture);
    }

    [Theory]
    [InlineData(2200)]
    [InlineData(5000000)]
    [InlineData(9000)]
    public void RemoveProfilePicture_NoPictureOrInvalidCustomerID_ReturnErrors(int customerID)
    {
        List<ValidationResult> errors = _customerService.RemoveProfilePicture(customerID);

        ProfilePicture profilePicture = _context.ProfilePictures.FirstOrDefault(x => x.CustomerID == customerID);

        Assert.NotNull(errors);
        Assert.Null(profilePicture);
    }

    private static IFormFile MockIFormFile(MagickFormat format, string extension)
    {
        using MagickImage mockImage = new("xc:purple", new MagickReadSettings()
        {
            Height = 600,
            Width = 800,
        });
        mockImage.Format = format;
        byte[] imageData = mockImage.ToByteArray();
        IFormFile file = new FormFile(new MemoryStream(imageData), 0, imageData.Length, "Data", $"photo.{extension}");
        return file;
    }
}