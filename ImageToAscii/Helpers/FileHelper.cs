namespace ImageToAscii.Helpers;

using System;
using System.IO;

public static class FileHelper
{
    public static string[] GetFileNamesInDirectory(string directory)
    {
        string[] result = null;

        string path = "./" + directory;

        if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(path))
        {
            result = Directory.GetFiles(path);
        }

        return result;
    }

    public static string GetImageUrl(string file)
    {
        var dir = Environment.CurrentDirectory;
        if (string.IsNullOrWhiteSpace(file)) return string.Empty;
        return $"./images/{file}";
    }
}