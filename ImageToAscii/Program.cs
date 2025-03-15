namespace ImageToAscii;

using System;
using System.Threading;

using Helpers;

using Models;

using Renderers;

internal class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var mode = InitiateModeSelection();

        var images = InitiateImageSelection(mode);

        var newConsoleRenderer = new ConsoleRendererV2(mode, images);

        var backgroundThread = new Thread(newConsoleRenderer.Start);

        backgroundThread.Start();

        Console.ReadKey();
    }

    public static RenderMode InitiateModeSelection()
    {
        while (true)
        {
            Console.Clear();
            var enumNames = Enum.GetNames(typeof(RenderMode));
            var index = 1;
            foreach (var item in enumNames)
            {
                var text = $"{index}. {item}";

                if (index <= enumNames.Length)
                    text += ", ";

                Console.Write(text);

                index++;
            }

            Console.WriteLine("Please select mode:");
            var inputString = Console.ReadLine();

            var input = int.Parse(inputString);

            if (input <= 0 && input >= index)
            {
                Console.WriteLine("Selected image number is invalid");
                Thread.Sleep(1000);
            }
            else
            {
                Enum.TryParse(enumNames[input - 1], out RenderMode mode);

                return mode;
            }
        }
    }

    public static string[] InitiateImageSelection(RenderMode mode)
    {
        if (mode == RenderMode.Image)
            while (true)
            {
                Console.Clear();
                var files = FileHelper.GetFileNamesInDirectory("images");

                int index = 1;
                foreach (var file in files)
                {
                    var charLocation = file.IndexOf('\\', StringComparison.Ordinal);
                    var fileName = file;
                    if (charLocation > 0)
                    {
                        fileName = file.Substring(charLocation + 1, (file.Length - charLocation - 1));
                    }
                    Console.WriteLine($"{index} - {fileName}");
                    index++;
                }

                Console.WriteLine("Please select image by typing in the number of image:");
                var inputString = Console.ReadLine();

                var input = int.Parse(inputString);

                if (input <= 0 && input >= index)
                {
                    Console.WriteLine("Selected image number is invalid");
                    Thread.Sleep(1000);
                }
                else
                {
                    var file = files[input - 1];
                    var charLocation = file.IndexOf('\\', StringComparison.Ordinal);
                    var fileName = file;
                    if (charLocation > 0)
                    {
                        fileName = file.Substring(charLocation + 1, (file.Length - charLocation - 1)); ;
                    }

                    return new[] { fileName };
                }
            }

        

        return FileHelper.GetFileNamesInDirectory("images");
    }
}