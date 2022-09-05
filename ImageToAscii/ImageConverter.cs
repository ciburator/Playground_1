using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ImageToAscii
{
    using System;
    using Helpers;

    internal class ImageConverter
    {
        private readonly string asciiVocab;
        private readonly int width;
        private readonly int height;

        public ImageConverter(string vocab, int width, int height)
        {
            this.asciiVocab = vocab;
            this.width = width;
            this.height = height;
        }

        public string[] GetStringImageMatrix(string url, bool reverseVocab = false)
        {
            Bitmap bitmap = new Bitmap(url);

            if (bitmap.Height > width || bitmap.Width > height)
            {
                bitmap = this.ResizeImage(bitmap);
            }

            var asciiLength = this.asciiVocab.Length - 1;
            var convertedVocab = this.asciiVocab;
            if (reverseVocab)
            {
                convertedVocab = this.asciiVocab.Reverse();
            }
            string[] asciiPicture = new string[bitmap.Height];

            for (int y = 0; y < bitmap.Height; y++)
            {
                string horizontalLine = string.Empty;
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);

                    var r = pixel.R;
                    var g = pixel.G;
                    var b = pixel.B;
                    var a = pixel.A;

                    var avg = (r + g + b) / 3;

                    var convertedAvg = this.ConvertToNewRange(avg, 255, asciiLength);

                    char symbol = convertedVocab[convertedAvg];
                    horizontalLine += symbol;
                }

                asciiPicture[y] = horizontalLine;
            }

            return asciiPicture;
        }

        public IEnumerable<string> CleanImage(string[] image) => image.Where(line => !string.IsNullOrWhiteSpace(line));

        private Bitmap ResizeImage(Bitmap bitmap)
        {
            Image image = bitmap;

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public void SaveFile(Bitmap image)
        {
            string fileName = "ResizedImage";

            int index = 0;
            bool fileSaved = false;

            while (!fileSaved)
            {
                if (!File.Exists(fileName + index))
                {
                    string finalName = fileName + index + ".png";
                    image.Save(finalName, ImageFormat.Png);
                    fileSaved = true;
                }
                else
                {
                    index++;
                }
            }
        }

        private int ConvertToNewRange(int value, int maxValue, int newRangeTo)
        {
            int originalRangeMax = maxValue;
            
            int newRangeMax = newRangeTo;

            var test = (value * 100) / originalRangeMax;

            int percentilePosition = (int)Math.Round((double)(value * 100) / originalRangeMax);

            int convertedValue = (int)Math.Round((double)(newRangeTo * percentilePosition) / 100);

            if (convertedValue < 0)
            {
                convertedValue = 0;
            }

            if (convertedValue > newRangeTo)
            {
                convertedValue = newRangeTo;
            }

            return convertedValue;
        }
    }
}
