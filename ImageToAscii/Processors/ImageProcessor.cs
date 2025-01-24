namespace ImageToAscii.Processors;

using System;
using System.Threading;
using Helpers;
using Converters;
using Models;
using System.Data.Common;

internal class ImageProcessor : BaseProcessor
{
    private readonly bool imageCycle;
    public ImageConverter ImageConverter { get; }

    public ImageProcessor(Configuration config, bool imageCycle = false) : base(config)
    {
        this.imageCycle = imageCycle;
        PixelMatrix = new string[
            Height + CombinedBorderOffset,
            Width + CombinedBorderOffset];

        PixelMatrix[0, 0] = Borders.UlCorner;
        PixelMatrix[0, OffsetWidth] = Borders.UrCorner;
        PixelMatrix[OffsetHeight, OffsetWidth] = Borders.LrCorner;
        PixelMatrix[OffsetHeight, 0] = Borders.LlCorner;

        ImageConverter = new ImageConverter(
            Config.AsciiVocab,
            Width,
            Height);
    } 

    public override void Init()
    {
        if (Config.EnableBorders)
        {
            char?[,] borders = new char?[OffsetHeight+1, OffsetWidth+1];

            borders[0, 0] = char.Parse(Borders.UlCorner);
            borders[0, OffsetWidth] = char.Parse(Borders.UrCorner);
            borders[OffsetHeight, OffsetWidth] = char.Parse(Borders.LrCorner);
            borders[OffsetHeight, 0] = char.Parse(Borders.LlCorner);

            for (int i = 1; i < OffsetWidth; i++)
            {
                borders[0, i] = char.Parse(Borders.Horizontal);
                borders[OffsetHeight, i] = char.Parse(Borders.Horizontal);
            }

            for (int i = 1; i < OffsetHeight; i++)
            {
                borders[i, 0] = char.Parse(Borders.Vertical);
                borders[i, OffsetWidth] = char.Parse(Borders.Vertical);
            }

            Draw(borders);
        }

        if (imageCycle)
        {
            string[] imageList = FileHelper.GetFileNamesInDirectory("images");
            bool reverseVocab = Config.ReverseVocab;

            foreach (var image in imageList)
            {
                Draw(ImageConverter.GetStringImageMatrix(image, reverseVocab));

                Thread.Sleep(Config.CycleDelay);
            }
        }
        else
        {
            var imageUrl = FileHelper.GetImageUrl(Config.ImageName);
            var stringImage = ImageConverter.GetStringImageMatrix(
                imageUrl, Config.ReverseVocab);

            Draw(
                stringImage,
                Config.ImageXOffset,
                Config.ImageYOffset);
        }

        Console.ReadKey();
    }

    public void Draw(char?[,] matrix, bool skipEmpty = true, bool overwritePrevious = false)
    {
        for (var row = 0; row < matrix.GetLength(0); row++)
        {
            for (var column = 0; column < matrix.GetLength(1); column++)
            {
                var symbol = matrix[row,column];

                if((symbol == null || symbol == char.Parse(" ")) && skipEmpty && !overwritePrevious)
                    continue;

                WritePos(column, row, symbol ?? char.Parse(" "));
            }
        }
    }

    public override void DrawBackground()
    {
        throw new NotImplementedException();
    }

    public override void Draw(string[] image, int offsetX = 0, int offsetY = 0)
    {
        if (image.Length > 0 && image[0].Length > 0)
        {
            Clear();

            int combinedOffsetX = BorderOffset + offsetX;
            int combinedOffsetY = BorderOffset + offsetY;

            int imageWidth = image[0].Length;
            int imageHeight = image.Length;

            for (int y = 0; y < image.Length; y++)
            {
                for (int x = 0; x < image[y].Length; x++)
                {
                    char currentPixel = image[y][x];
                    // Can be refactored to update multiple chars per line
                    if (currentPixel != ' ')
                    {
                        var posX = x + combinedOffsetX;
                        var posY = y + combinedOffsetY;

                        CursorPos(posX, posY);
                        Write(currentPixel);
                    }
                }
            }
        }
    }

    private void WritePos(int x, int y, char symbol)
    {
        CursorPos(x, y + Config.OffsetFromTop);
        Write(symbol);
    }
}