namespace ConsoleHandler.Models;

internal class ConsoleCanvasConfigurationExtended : ConsoleCanvasConfiguration
{
    private const int DefaultBorderHorizontalOffset = 1;
    private const int DefaultBorderVerticalOffset = 1;

    public ConsoleCanvasConfigurationExtended(
        ConsoleCanvasConfiguration configuration,
        int verticalOffset,
        int width)
    {
        this.Name = configuration.Name;
        this.Height = configuration.ShowBorder ? configuration.Height + 2 : configuration.Height;
        this.Position = configuration.Position;
        this.ShowBorder = configuration.ShowBorder;
        this.DefaultCursorPosition = configuration.DefaultCursorPosition;

        this.Width = width;
        this.Data = new char?[Height, Width];

        ImageWidth = width - (ShowBorder ? 2 : 1);
        ImageHeight = Height - (ShowBorder ? 2 : 1);

        PrepareOffset(verticalOffset);

        PrepareBorders();
    }

    /// <summary>
    /// Horizontal offset
    /// Mainly used for start position
    /// </summary>
    public int HorizontalOffset { get; set; }

    /// <summary>
    /// Offset containing
    /// Mainly used for start position
    /// </summary>
    public int VerticalOffset { get; set; }

    public int ImageHorizontalOffset { get; set; }
    public int ImageVerticalOffset { get; set; }

    /// <summary>
    /// This is Width with offset for array starting position and also border offset
    /// </summary>
    public int ImageWidth { get; set; }

    /// <summary>
    /// This is with Height offset for array starting position and also border offset
    /// </summary>
    public int ImageHeight { get; set; }

    /// <summary>
    /// Total canvas width
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Canvas matrix
    /// </summary>
    public char?[,] Data { get; set; } // can be used to verify what is drawn or should be drawn and then redraw only changes

    private void PrepareOffset(int verticalOffset)
    {
        HorizontalOffset = 0;
        VerticalOffset = verticalOffset;
        ImageHorizontalOffset = ImageVerticalOffset = ShowBorder ? 1 : 0;
    }

    private char?[,] PrepareBorders()
    {
        if (ShowBorder)
        {
            Data[0, 0] = char.Parse(Borders.UlCorner);
            Data[0, Width - 1] = char.Parse(Borders.UrCorner);
            Data[Height - 1, Width - 1] = char.Parse(Borders.LrCorner);
            Data[Height - 1, 0] = char.Parse(Borders.LlCorner);

            for (int i = 1; i < Width - 1; i++)
            {
                Data[0, i] = char.Parse(Borders.Horizontal);
                Data[Height - 1, i] = char.Parse(Borders.Horizontal);
            }

            for (int i = 1; i < Height - 1; i++)
            {
                Data[i, 0] = char.Parse(Borders.Vertical);
                Data[i, Width - 1] = char.Parse(Borders.Vertical);
            }
        }

        return Data;
    }
}