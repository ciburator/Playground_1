using System.IO;

namespace ImageToAscii.Helpers
{
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
            if (string.IsNullOrWhiteSpace(file)) return string.Empty;
            return $"./images/{file}";
        }
    }
}