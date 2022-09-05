using System;
using ImageToAscii.Models;

namespace ImageToAscii.Renderers
{
    public abstract class BaseRenderer
    {
        public Configuration Config { get; }

        protected int Width;
        protected int Height;

        protected int OffsetWidth;
        protected int OffsetHeight;

        // Border symbol width
        protected int BorderOffset = 1;
        // Combined border width
        protected int CombinedBorderOffset = 2;

        protected string[,] PixelMatrix;

        protected BaseRenderer(Configuration config)
        {
            Config = config;

            this.Height = config.Height;
            this.Width = config.Width;

            this.OffsetHeight = config.HeightOffset;
            this.OffsetWidth = config.WidthOffset;

            this.OffsetHeight = this.Height + this.BorderOffset;
            this.OffsetWidth = this.Width + this.BorderOffset;
        }

        public abstract void Init();

        public abstract void DrawBorder();

        public abstract void DrawBackground();

        public abstract void Draw(string[] image, int offsetX = 0, int offsetY = 0);

        /// <summary>
        /// Clear only picture space not borders
        /// </summary>
        protected void Clear()
        {
            string line = new string(' ', this.Width);

            for (int row = 1; row < this.OffsetHeight; row++)
            {
                this.CursorPos(1, row);
                this.WriteLine(line);
            }
        }

        /// <summary>
        /// Completely clears console
        /// </summary>
        protected void CompleteClear() => Console.Clear();

        /// <summary>
        /// Writes pixel in last line cursor was
        /// </summary>
        /// <param name="pixel"></param>
        protected void Write(char pixel) => Console.Write(pixel);

        /// <summary>
        /// Writes complete line in current selected line
        /// </summary>
        /// <param name="line">text string to write</param>
        protected void WriteLine(string line) => Console.Write(line);

        /// <summary>
        /// Set console's cursor position
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        protected void CursorPos(int x, int y) => Console.SetCursorPosition(x, y);


    }
}