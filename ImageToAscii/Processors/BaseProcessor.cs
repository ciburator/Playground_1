namespace ImageToAscii.Processors;

using System;
using Models;

public abstract class BaseProcessor
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

    protected BaseProcessor(Configuration config)
    {
        Config = config;

        Height = config.Height;
        Width = config.Width;

        OffsetHeight = Height + BorderOffset;
        OffsetWidth = Width + BorderOffset;
    }

    public abstract void Init();

    public abstract void DrawBackground();

    public abstract void Draw(string[] image, int offsetX = 0, int offsetY = 0);

    /// <summary>
    /// Clear only picture space not borders
    /// </summary>
    protected void Clear()
    {
        string line = new string(' ', Width);

        for (int row = 1; row < OffsetHeight; row++)
        {
            CursorPos(1, row);
            WriteLine(line);
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