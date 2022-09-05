using System.IO;

namespace ImageToAscii.Helpers
{
    public class FileHelper
    {
        public FileHelper() {}

        public string[] GetFileNamesInDirectory(string directory)
        {
            string[] result = null;

            string path = "./" + directory;

            if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(path))
            {
                result = Directory.GetFiles(path);
            }

            return result;
        }
    }
}