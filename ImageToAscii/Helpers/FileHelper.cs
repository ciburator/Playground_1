namespace ImageToAscii.Helpers;

using System;
using System.IO;

public static class FileHelper
{
    public static string[] GetFileNamesInDirectory(string directory)
    {
        string[] result = null;

        string path = ".\\" + directory;

        if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(path))
        {
            result = Directory.GetFiles(path);
        }

        return result;
    }

    public static string GetImageUrl(string file)
    {
        var dir = Environment.CurrentDirectory;

        if (file.Contains(".\\images\\"))
            return Path.Join(dir, file.Substring(1, file.Length - 1));

        if (file.Contains("\\images\\"))
            return Path.Join(dir, file);
        
        if (string.IsNullOrWhiteSpace(file)) return string.Empty;
        return Path.Join(dir, $"\\images\\{file}");
    }
}