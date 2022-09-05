using ImageToAscii.Models;

namespace ImageToAscii
{
    using System;

    internal class Renderer
    {
        private readonly int height;
        private readonly int width;

        private readonly int offsetHeight;
        private readonly int offsetWidth;

        // Border symbol width
        private readonly int borderOffset = 1;
        // Combined border width
        private readonly int combinedBorderOffset = 2;
        private readonly string[,] pixelMatrix;

        public Renderer(
            int width,
            int height)
        {
            this.height = height;
            this.width = width;

            this.offsetHeight = height + this.borderOffset;
            this.offsetWidth = width + this.borderOffset;

            this.pixelMatrix = new string[this.height + combinedBorderOffset, this.width + combinedBorderOffset];

            this.pixelMatrix[0, 0] = Borders.UlCorner;
            this.pixelMatrix[0, this.offsetWidth] = Borders.UrCorner;
            this.pixelMatrix[this.offsetHeight, this.offsetWidth] = Borders.LrCorner;
            this.pixelMatrix[this.offsetHeight, 0] = Borders.LlCorner;
            this.PrepareBorder();
        }

        public void DrawBorder(string[] image = null)
        {
            CompleteClear();

            bool drawImage = image is { Length: > 0 };

            for (int row = 0; row <= this.offsetHeight; row++)
            {
                string line = "";

                for (int column = 0; column <= this.offsetWidth; column++)
                {

                    if (row == 0)
                    {
                        if (column == 0) //Corner LT
                        {
                            line += Borders.UlCorner;
                        }
                        else if (column == (this.offsetWidth)) //Corner RT
                        {
                            line += Borders.UrCorner;
                        }
                        else //Top line
                        {
                            line += Borders.Horizontal;
                        }
                    }
                    else if (row == (this.offsetHeight))
                    {
                        if (column == 0) //Corner LB
                        {
                            line += Borders.LlCorner;
                        }
                        else if (column == (this.offsetWidth)) //Corner RB
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
                        else if (column == (this.offsetWidth)) //Right line
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

        public void DrawImage(string[] image, int offsetX = 0, int offsetY = 0)
        {
            if (image.Length > 0 && image[0].Length > 0)
            {
                this.Clear();

                int combinedOffsetX = borderOffset + offsetX;
                int combinedOffsetY = borderOffset + offsetY;

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

        /// <summary>
        /// Clear only picture space not borders
        /// </summary>
        public void Clear()
        {
            string line = new string(' ', this.width);

            for (int row = 1; row < this.offsetHeight; row++)
            {
                this.CursorPos(1, row);
                this.WriteLine(line);
            }
        }

        /// <summary>
        /// Completely clears console
        /// </summary>
        public void CompleteClear() => Console.Clear();

        /// <summary>
        /// Writes pixel in last line cursor was
        /// </summary>
        /// <param name="pixel"></param>
        private void Write(char pixel) => Console.Write(pixel);

        /// <summary>
        /// Writes complete line in current selected line
        /// </summary>
        /// <param name="line">text string to write</param>
        private void WriteLine(string line) => Console.Write(line);

        /// <summary>
        /// Set console's cursor position
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        private void CursorPos(int x, int y) => Console.SetCursorPosition(x, y);

        private void PrepareBorder()
        {

        }
    }
}
