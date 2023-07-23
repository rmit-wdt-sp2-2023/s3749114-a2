using ImageMagick;

namespace CustomerApplication.Utilities;

public static class ProfileImageProcessor
{




    public static bool PermittedExension(string fileName)
    {
        string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".heic" };
        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
        {
            return false;
        }
        return true;
    }

    public static void ConvertToJpg(string filePath)
    {
        using var image = new MagickImage(filePath);

    }

    public static void Resize(string directory, string imageName, string newName)
    {
        string filePath = Path.Combine(directory, imageName);

        using MagickImage image = new(filePath);

        var size = new MagickGeometry(400, 400);

        image.Resize(size);

        image.Format = MagickFormat.Jpg;

        string newfilepath = Path.Combine(directory, newName);
        image.Write(newfilepath + ".jpg");

    }
}