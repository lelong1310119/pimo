using Microsoft.VisualBasic.FileIO;

namespace PismoWebInput.Core.Infrastructure.Common.IO;

public static class DirectoryHelper
{
    public static string CreateIfNotExists(string directory)
    {
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        return directory;
    }

    public static void DeleteDirectory(string root)
    {
        if (FileSystem.DirectoryExists(root)) FileSystem.DeleteDirectory(root, DeleteDirectoryOption.DeleteAllContents);
    }

    public static void DeleteFile(string file)
    {
        if (File.Exists(file)) File.Delete(file);
    }
}