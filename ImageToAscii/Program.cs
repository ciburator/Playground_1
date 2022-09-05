using ImageToAscii.Helpers;
using System.Linq;
using System.Threading;

namespace ImageToAscii
{
    using System;

    internal class Program
    {
        private static int width = 200;
        private static int height = 80;

        private static int widthOffset = 3;
        private static int heightOffset = 3;

        private static string ImageName = "ciblogo3.png";

        [STAThread]
        static void Main(string[] args)
        {
            var renderer = new Renderer(width, height);
            var fileHelper = new FileHelper();

            Console.SetWindowSize(width + widthOffset, height + heightOffset);

            var imageRenderer = new ImageConverter($"./images/{ImageName}", width, height);

            var stringImage = imageRenderer.GetStringImageMatrix(true);

            var cleanedStringImage = stringImage.Where(line => !string.IsNullOrWhiteSpace(line));

            //Clipboard.SetText(String.Join(Environment.NewLine, cleanedStringImage));

            renderer.DrawBorder(stringImage);

            renderer.DrawImage(stringImage);

            int milliseconds = 2000;

            Thread.Sleep(milliseconds);

            string[] ImageList = fileHelper.GetFileNamesInDirectory("images");

            foreach (var image in ImageList)
            {
                var imgRend = new ImageConverter(image, width, height);

                var imageMatrix = imgRend.GetStringImageMatrix(true);

                renderer.DrawImage(imageMatrix);

                Thread.Sleep(milliseconds);
            }

            Console.ReadKey();

            //while (true)
            //{
            //    if (!stopRenderer)
            //    {
            //        renderer.DrawFrame();
            //    }

            //    if (Console.KeyAvailable)
            //    {
            //        var key = Console.ReadKey(true);

            //        switch (key.Key)
            //        {
            //            case ConsoleKey.Escape:
            //                stopRenderer = true;
            //                Console.WriteLine("Stopped");
            //                break;
            //        }
            //    }
            //}
        }
    }
}
