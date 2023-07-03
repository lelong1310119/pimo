namespace PismoWebInput.Core.Infrastructure.Common.IO;

public static class FileHelper
{
    public static bool DeleteIfExists(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) return false;

        var exists = File.Exists(filePath);
        if (exists) File.Delete(filePath);
        return exists;
    }
}