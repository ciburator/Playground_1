using System.Threading;

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

        private string ulCorner = "╔";
        private string llCorner = "╚";
        private string urCorner = "╗";
        private string lrCorner = "╝";
        private string vertical = "║";
        private string horizontal = "═";

        public Renderer(
            int width,
            int height)
        {
            this.height = height;
            this.width = width;

            this.offsetHeight = height + this.borderOffset;
            this.offsetWidth = width + this.borderOffset;

            this.pixelMatrix = new string[this.height + combinedBorderOffset, this.width + combinedBorderOffset];

            this.pixelMatrix[0, 0] = ulCorner;
            this.pixelMatrix[0, this.offsetWidth] = urCorner;
            this.pixelMatrix[this.offsetHeight, this.offsetWidth] = lrCorner;
            this.pixelMatrix[this.offsetHeight, 0] = llCorner;
            this.PrepareBorder();
        }

        public void DrawBorder(string[] image = null)
        {
            Console.Clear();

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
                            line += this.ulCorner;
                        }
                        else if (column == (this.offsetWidth)) //Corner RT
                        {
                            line += this.urCorner;
                        }
                        else //Top line
                        {
                            line += this.horizontal;
                        }
                    }
                    else if (row == (this.offsetHeight))
                    {
                        if (column == 0) //Corner LB
                        {
                            line += this.llCorner;
                        }
                        else if (column == (this.offsetWidth)) //Corner RB
                        {
                            line += this.lrCorner;
                        }
                        else //Bot line
                        {
                            line += this.horizontal;
                        }
                    }
                    else
                    {
                        if (column == 0) //Left line
                        {
                            line += this.vertical;
                        }
                        else if (column == (this.offsetWidth)) //Right line
                        {
                            line += this.vertical;
                        }
                        else
                        {
                            //var correctedRow = row != 0 ? row - 1 : row;
                            //var correctColumn = column != 0 ? column - 1 : column;

                            //if (drawImage && 
                            //    (image.Length + 1) > row && 
                            //    (image[correctedRow].Length + 1) > column)
                            //{
                            //    line += image[correctedRow][correctColumn];
                            //}
                            //else
                            //{
                            line += ' ';
                            //}
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

        public void Clear()
        {
            string line = new string(' ', this.width);

            for (int row = 1; row < this.offsetHeight; row++)
            {
                this.CursorPos(1, row);
                this.WriteLine(line);
            }
        }

        private void Write(char pixel) => Console.Write(pixel);
        private void WriteLine(string line) => Console.Write(line);
        private void CursorPos(int x, int y) => Console.SetCursorPosition(x, y);

        private void PrepareBorder()
        {

        }
    }
}
