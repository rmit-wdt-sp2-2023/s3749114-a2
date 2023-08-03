using System.ComponentModel.DataAnnotations;
using BankLibrary.Models;
using BankLibrary.Validation;
using ImageMagick;
using BankLibrary.Data;

namespace CustomerApplication.Services;

public class CustomerService
{
    private readonly BankContext _context;

    public CustomerService(BankContext context) => _context = context;

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

        Customer customerUpdates = new()
        {
            CustomerID = customerID,
            Name = name,
            TFN = TFN,
            Address = address,
            City = city,
            State = state?.ToUpper(),
            PostCode = postCode,
            Mobile = mobile,
        };

        if (!ValidationMethods.Validate(customerUpdates, out errors))
            return errors;

        customer.Name = name;
        customer.TFN = TFN;
        customer.Address = address;
        customer.City = city;
        customer.State = state?.ToUpper();
        customer.PostCode = postCode;
        customer.Mobile = mobile;

        _context.Customers.Update(customer);
        _context.SaveChanges();

        return null;
    }

    public ProfilePicture GetProfilePicture(int customerID)
    {
        ProfilePicture profilePicture = GetCustomerProfilePicture(customerID);

        if (profilePicture is null || profilePicture.Image.Length <= 0)
            return DefaultProfilePicture();

        return profilePicture;
    }

    public ValidationResult UploadProfilePicture(int customerID, IFormFile profileImage)
    {
        List<string> member = new() { "ProfileImage" };

        if (profileImage is null || profileImage.Length <= 0)
            return new ValidationResult("You must select an image.", member);

        Customer customer = GetCustomer(customerID);

        if (customer is null)
            return new ValidationResult("Update unsuccessful. Unable to find customer.", member);

        // check extension

        string ext = Path.GetExtension(profileImage.FileName).ToLowerInvariant();

        string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".heic" };

        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            return new ValidationResult("Invalid file type.", member);

        byte[] imageData = Array.Empty<byte>();

        try
        {
            using Stream stream = profileImage.OpenReadStream();
            using MagickImage image = new(stream);
            image.Format = MagickFormat.Jpg;
            image.Resize(new MagickGeometry(400, 400));
            image.Quality = 100;
            image.Alpha(AlphaOption.Remove);
            image.BackgroundColor = new MagickColor("#FFFFFF");
            imageData = image.ToByteArray();
        }
        catch (MagickCoderErrorException)
        {
            return new ValidationResult("Upload failed. Image may be corrupt. Try a different image.", member);
        }
        catch (MagickException)
        {
            return new ValidationResult("Error processing. Try again or choose a different image.", member);
        }
        catch (Exception)
        {
            return new ValidationResult("Upload failed. Try again or choose a different image.", member);
        }

        ProfilePicture profilePicture = GetCustomerProfilePicture(customerID);

        if (profilePicture is null)
        {
            _context.ProfilePictures.Add(new ProfilePicture()
            {
                CustomerID = customerID,
                Image = imageData,
                ContentType = "img/jpg",
                FileName = $"{customerID}-profile-picture.jpg"
            });
        }
        else
        {
            profilePicture.Image = imageData;
            _context.ProfilePictures.Update(profilePicture);
        }

        _context.SaveChanges();

        return null;
    }

    public List<ValidationResult> RemoveProfilePicture(int customerID)
    {
        List<ValidationResult> errors = new();
        ProfilePicture profilePicture = GetCustomerProfilePicture(customerID);

        if (profilePicture is null)
        {
            errors.Add(new ValidationResult("No profile picture to remove.", new List<string>() { "ProfilePicture" }));
            return errors;
        }

        _context.ProfilePictures.Remove(profilePicture);
        _context.SaveChanges();

        return null;
    }

    private static ProfilePicture DefaultProfilePicture()
    {
        ProfilePicture profilePicture = new()
        {
            CustomerID = 0,
            ContentType = "img/jpg",
            FileName = "default.jpg",
            Image = Array.Empty<byte>()
        };
        try
        {
            string filePath = $"{Directory.GetCurrentDirectory()}/wwwroot/Content/Images/default.jpg";

            FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);

            using BinaryReader reader = new(fileStream);

            byte[] image = new byte[reader.BaseStream.Length];

            for (int i = 0; i < reader.BaseStream.Length; i++)
                image[i] = reader.ReadByte();

            profilePicture.Image = image;

            return profilePicture;
        }
        catch (Exception)
        {
            return profilePicture;
        }
    }

    private ProfilePicture GetCustomerProfilePicture(int customerID) =>
        _context.ProfilePictures.FirstOrDefault(x => x.CustomerID == customerID);
}