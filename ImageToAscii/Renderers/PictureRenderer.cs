using System.Threading;
using ImageToAscii.Helpers;
using ImageToAscii.Models;

namespace ImageToAscii.Renderers
{
    using System;

    internal class PictureRenderer : BaseRenderer
    {
        private readonly bool imageCycle;
        public ImageConverter ImageConverter { get; }

        public PictureRenderer( Configuration config, bool imageCycle = false): base(config)
        {
            this.imageCycle = imageCycle;
            this.PixelMatrix = new string[
                this.Height + this.CombinedBorderOffset, 
                this.Width + this.CombinedBorderOffset];

            this.PixelMatrix[0, 0] = Borders.UlCorner;
            this.PixelMatrix[0, this.OffsetWidth] = Borders.UrCorner;
            this.PixelMatrix[this.OffsetHeight, this.OffsetWidth] = Borders.LrCorner;
            this.PixelMatrix[this.OffsetHeight, 0] = Borders.LlCorner;

            this.ImageConverter = new ImageConverter(
                this.Config.AsciiVocab, 
                this.Width, 
                this.Height);
        }

        public override void Init()
        {
            if (this.Config.EnableBorders)
            {
                this.DrawBorder();
            }
            

            if (this.imageCycle)
            {
                string[] imageList = FileHelper.GetFileNamesInDirectory("images");
                bool reverseVocab = this.Config.ReverseVocab;

                foreach (var image in imageList)
                {
                    this.Draw(this.ImageConverter.GetStringImageMatrix(image, reverseVocab));

                    Thread.Sleep(this.Config.CycleDelay);
                }
            }
            else
            {
                var imageUrl = FileHelper.GetImageUrl(this.Config.ImageName);
                var stringImage = this.ImageConverter.GetStringImageMatrix(
                    imageUrl, this.Config.ReverseVocab);

                this.Draw(
                    stringImage, 
                    this.Config.ImageXOffset, 
                    this.Config.ImageYOffset);
            }

            Console.ReadKey();
        }

        public override void DrawBorder()
        {
            CompleteClear();

            for (int row = 0; row <= this.OffsetHeight; row++)
            {
                string line = "";

                for (int column = 0; column <= this.OffsetWidth; column++)
                {

                    if (row == 0)
                    {
                        if (column == 0) //Corner LT
                        {
                            line += Borders.UlCorner;
                        }
                        else if (column == (this.OffsetWidth)) //Corner RT
                        {
                            line += Borders.UrCorner;
                        }
                        else //Top line
                        {
                            line += Borders.Horizontal;
                        }
                    }
                    else if (row == (this.OffsetHeight))
                    {
                        if (column == 0) //Corner LB
                        {
                            line += Borders.LlCorner;
                        }
                        else if (column == (this.OffsetWidth)) //Corner RB
                        {
                            line += Borders.LrCorner;
                        }
                        else //Bot line
                        {
                            line += Borders.Horizontal;
                        }
                    }
                    else
                    {
                        if (column == 0) //Left line
                        {
                            line += Borders.Vertical;
                        }
                        else if (column == (this.OffsetWidth)) //Right line
                        {
                            line += Borders.Vertical;
                        }
                        else
                        {
                            line += ' ';
                        }
                    }
                }

                Console.WriteLine(line);
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
                this.Clear();

                int combinedOffsetX = this.BorderOffset + offsetX;
                int combinedOffsetY = this.BorderOffset + offsetY;

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

                            this.CursorPos(posX, posY);
                            this.Write(currentPixel);
                        }
                    }
                }
            }
        }
    }
}
