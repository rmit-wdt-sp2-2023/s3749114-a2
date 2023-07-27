using System.ComponentModel.DataAnnotations;
using BankLibrary.Models;
using BankLibrary.Validation;
using ImageMagick;
using BankLibrary.Data;

namespace CustomerApplication.Services;

public class CustomerService
{
    private readonly BankContext _context;

    private readonly string _directory;

    public CustomerService(BankContext context, IWebHostEnvironment webHostEnvironment)
	{
        _context = context;
        _directory = Path.Combine(webHostEnvironment.WebRootPath, "ProfilePictures");
    }

    public Customer GetCustomer(int customerID) => _context.Customers.FirstOrDefault(c => c.CustomerID == customerID);

    public List<ValidationResult> UpdateCustomer(int customerID, string name, string TFN,
        string address, string city, string state, string postCode, string mobile)
    {
        Customer customer = GetCustomer(customerID);
        List<ValidationResult> errors = new();

        if (customer is null)
        {
            errors.Add(new ValidationResult("Update failed. Can't find customer.", new List<string>() { "Other" }));
            return errors;
        }
        customer.Name = name;
        customer.TFN = TFN;
        customer.Address = address;
        customer.City = city;
        customer.State = state?.ToUpper();
        customer.PostCode = postCode;
        customer.Mobile = mobile;

        if (!ValidationMethods.Validate(customer, out errors))
            return errors;

        _context.Customers.Update(customer);
        _context.SaveChanges();

        return null;
    }

    public (ValidationResult, string fileName) UploadProfilePicture(int customerID, IFormFile profileImage)
    {
        if (profileImage is null)
            return (new ValidationResult("You must select an image.", new List<string>() { "ProfileImage" }), null);

        Customer customer = GetCustomer(customerID);

        if (customer is null)
            return (new ValidationResult(
                "Update unsuccessful. Unable to find customer.", new List<string>() { "ProfileImage" }), null);

        string ext = Path.GetExtension(profileImage.FileName).ToLowerInvariant();

        string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".heic" };

        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            return (new ValidationResult("Invalid file type.", new List<string>() { "ProfileImage" }), null);

        string tempFilePath = Path.Combine(_directory, $"{customerID}-temp{ext}");
        string newFileName = $"{Guid.NewGuid()}-{customerID}.jpg";
        string newFilePath = Path.Combine(_directory, newFileName);

        try
        {
            using FileStream fileStream = new(tempFilePath, FileMode.Create);
            profileImage.CopyTo(fileStream);

            using MagickImage image = new(tempFilePath);
            image.Resize(new MagickGeometry(400, 400));
            image.Format = MagickFormat.Jpg;
            image.Quality = 100;
            image.Alpha(AlphaOption.Remove);
            image.BackgroundColor = new MagickColor("#FFFFFF");
            image.Write(newFilePath);

            File.Delete(tempFilePath);
        }
        catch (MagickCoderErrorException)
        {
            return (new ValidationResult("Upload failed. Image may be corrupt. Try a different image.",
                new List<string>() { "ProfileImage" }), null);
        }
        catch (IOException)
        {
            return (new ValidationResult("Upload failed. Image does not exist.",
                new List<string>() { "ProfileImage" }), null);
        }
        catch (MagickException)
        {
            return (new ValidationResult("Error processing image. Try again or choose a different image.",
                new List<string>() { "ProfileImage" }), null);
        }
        catch (Exception)
        {
            return (new ValidationResult("Upload failed. Try again or choose a different image.",
                new List<string>() { "ProfileImage" }), null);
        }
        finally
        {
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }
        customer.ProfilePicture = newFileName;

        _context.Customers.Update(customer);
        _context.SaveChanges();

        return (null, newFileName);
    }

    public List<ValidationResult> RemoveProfilePicture(int customerID)
    {
        List<ValidationResult> errors = new();
        Customer customer = GetCustomer(customerID);

        if (customer is null)
        {
            errors.Add(new ValidationResult("Could not update. Can't find customer.", new List<string>() { "Other" }));
            return errors;
        }
        if (customer.ProfilePicture is null)
        {
            errors.Add(new ValidationResult("No profile picture to remove.", new List<string>() { "ProfilePicture" }));
            return errors;
        }
        string filePath = Path.Combine(_directory, customer.ProfilePicture);

        try
        {
            File.Delete(filePath);
        }
        catch (IOException)
        {
            errors.Add(new ValidationResult(
                "Could not find profile picture to delete.", new List<string>() { "ProfilePicture" }));
        }
        catch (Exception)
        {
            errors.Add(new ValidationResult(
                "Couldn't remove profile picture. Try again later or contact an admin.",
                new List<string>() { "ProfilePicture" }));
        }
        customer.ProfilePicture = null;

        _context.Customers.Update(customer);
        _context.SaveChanges();

        return null;
    }
}