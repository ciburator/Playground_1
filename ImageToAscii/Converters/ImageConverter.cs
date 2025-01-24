namespace ImageToAscii.Converters;

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using System;
using Helpers;

internal class ImageConverter
{
    private readonly string _asciiVocab;
    private readonly int _width;
    private readonly int _height;

    public ImageConverter(string vocab, int width, int height)
    {
        _asciiVocab = vocab;
        _width = width;
        _height = height;
    }

    public string[] GetStringImageMatrix(string url, bool reverseVocab = false)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            string[] blankPicture = new string[_height];
            for (int i = 0; i < _height; i++)
            {
                var horizontalLine = string.Empty;
                for (int j = 0; j < _width; j++)
                {
                    horizontalLine += ".";
                }

                blankPicture[i] = horizontalLine;
            }

            return blankPicture;
        }

        Bitmap bitmap = new Bitmap(url);

        if (bitmap.Height > _width || bitmap.Width > _height)
        {
            bitmap = ResizeImage(bitmap);
        }

        var asciiLength = _asciiVocab.Length - 1;
        var convertedVocab = _asciiVocab;
        if (reverseVocab)
        {
            convertedVocab = _asciiVocab.Reverse();
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

                var convertedAvg = ConvertToNewRange(avg, 255, asciiLength);

                char symbol = convertedVocab[convertedAvg];
                horizontalLine += symbol;
            }

            asciiPicture[y] = horizontalLine;
        }

        return asciiPicture;
    }

    public char?[,] GetCharMatrix(string[] imageString)
    {
        char?[,] result = new char?[_height, _width];
        for (int y = 0; y < imageString.Length; y++)
        {
            var charArray = imageString[y].ToCharArray();
            for (int x = 0; x < charArray.Length; x++)
            {
                result[y, x] = charArray[x];
            }
        }

        return result;
    }

    public IEnumerable<string> CleanImage(string[] image) => image.Where(line => !string.IsNullOrWhiteSpace(line));

    private Bitmap ResizeImage(Bitmap bitmap)
    {
        Image image = bitmap;

        var destRect = new Rectangle(0, 0, _width, _height);
        var destImage = new Bitmap(_width, _height);

        destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        using var graphics = Graphics.FromImage(destImage);

        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        using var wrapMode = new ImageAttributes();
        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

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