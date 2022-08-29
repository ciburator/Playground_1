using System.IO;

namespace ImageToAscii
{
    public class FileHelper
    {
        public FileHelper() {}

        public string[] GetFilenamesInDirectory(string directory)
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