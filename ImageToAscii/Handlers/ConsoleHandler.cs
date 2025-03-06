namespace ImageToAscii.Handlers;

using ImageToAscii.Models;
using System.Collections.Generic;
using System;
using System.Linq;

internal class ConsoleHandler
{
    private const int DefaultWindowHorizontalBorderOffset = 3;
    private const int DefaultWindowVerticalBorderOffset = 3;

    private const int DefaultBorderHorizontalOffset = 1;
    private const int DefaultBorderVerticalOffset = 1;

    public bool IsInitialized { get; private set; }

    private readonly int _horizontalOffset = 3;
    private readonly int _verticalOffset = 3;

    private readonly int _height;
    private readonly int _width;

    private IList<ConsoleCanvasConfigurationExtended> _canvasList;

    public ConsoleHandler(ConsoleConfiguration config)
    {
        if (config.CanvasList.Count == 0)
            throw new Exception("No canvas provided");

        _width = config.Width;

        OrderCanvasList(config.CanvasList);

        InitializeCanvas();

        _height = _canvasList.Select(item => item.Height).Sum();

        Console.SetWindowSize(
            _width + DefaultWindowHorizontalBorderOffset,
            _height + DefaultWindowVerticalBorderOffset);

        DrawBorders();

        IsInitialized = true;
    }

    public void Clear(int? canvasPosition = null, string? canvasName = null)
    {
        Check();

        var canvas = GetCanvas(canvasPosition, canvasName);

        for (int y = 0; y < canvas.Height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                canvas.Data[x, y] = ' ';
                Write(x + canvas.HorizontalOffset, y + canvas.VerticalOffset, ' ');
            }
        }
    }

    public void WriteLine(string text, int? canvasPosition = null, string? canvasName = null)
    {
        Check();
        var canvas = GetCanvas(canvasPosition, canvasName);

        if (canvas.Data[canvas.HorizontalOffset, canvas.VerticalOffset] != ' ')
        {
            canvas.VerticalOffset++;

            // we overwrite existing in overflow scenario
            if (canvas.VerticalOffset == (canvas.ShowBorder ? canvas.Height - 1 : canvas.Height))
                GetCanvasOffset(canvas);
        }

        var xPos = canvas.HorizontalOffset;
        foreach (char letter in text)
        {
            canvas.Data[xPos, canvas.VerticalOffset] = letter;
            xPos++;
        }

        Console.SetCursorPosition(canvas.HorizontalOffset, canvas.VerticalOffset);
        Console.WriteLine(text);
    }

    public void Write(int x, int y, char symbol, int? canvasPosition = null, string? canvasName = null)
    {
        Check();

        var canvas = GetCanvas(canvasPosition, canvasName);
        canvas.Data[x, y] = symbol;

        Write(x + canvas.HorizontalOffset, y + canvas.VerticalOffset, symbol);
    }

    private void Write(int x, int y, char symbol)
    {
        Console.SetCursorPosition(x, y);
        Console.Write(symbol);
    }

    public ConsoleCanvasConfigurationExtended GetCanvas(int? canvasPosition = null, string? canvasName = null)
    {
        if (canvasPosition != null)
            return GetCanvasByPosition((int)canvasPosition);

        if (!string.IsNullOrWhiteSpace(canvasName))
            return GetCanvasByName(canvasName);

        if (_canvasList.Count == 1)
            return _canvasList[0];

        throw new Exception("Please provide at least one identifier");
    }

    private ConsoleCanvasConfigurationExtended GetCanvasByName(string canvasName)
    {
        if (string.IsNullOrWhiteSpace(canvasName))
            throw new Exception("Please provide name");

        return _canvasList.First(item => String.Equals(item.Name, canvasName, StringComparison.CurrentCultureIgnoreCase));
    }

    private ConsoleCanvasConfigurationExtended GetCanvasByPosition(int canvasPosition)
    {
        if (canvasPosition >= _canvasList.Count)
            throw new Exception("Out of bounds position");

        return _canvasList.First(item => item.Position == canvasPosition);
    }

    private void GetCanvasOffset(
        ConsoleCanvasConfigurationExtended canvas)
    {
        var offsetMultiplier = (int)(canvas.Position! + 1);

        var verticalOffset = _canvasList
            .Where(item => item.Position < canvas.Position)
            .Sum(item => item.Height);

        canvas.HorizontalOffset = canvas.ShowBorder ? DefaultBorderHorizontalOffset : DefaultBorderHorizontalOffset - 1;
        canvas.VerticalOffset = canvas.ShowBorder
            ? DefaultBorderVerticalOffset * offsetMultiplier + verticalOffset
            : (DefaultBorderVerticalOffset * offsetMultiplier + verticalOffset) - 1;
    }

    private void Check()
    {
        if (!IsInitialized)
            throw new Exception("Please initialize first");
    }

    private void InitializeCanvas()
    {
        foreach (var canvas in _canvasList)
        {
            GetCanvasOffset(canvas);

            //probably need to offset usable width
            canvas.Width = _width;
            canvas.Data = new char?[canvas.Width, canvas.Height];
        }
    }

    private void OrderCanvasList(IList<ConsoleCanvasConfiguration> canvasList)
    {

        var convertedCanvasList = canvasList.Select(item => new ConsoleCanvasConfigurationExtended(item)).ToList();

        if (canvasList.Count <= 1) return;

        _canvasList = convertedCanvasList.OrderBy(item => item.Position).ToList();



        var maxPosition = _canvasList.Max(item => item.Position);

        //No further ordering required
        if (_canvasList.FirstOrDefault(item => item.Position == null) == null)
            return;

        var index = maxPosition + 1;

        if (maxPosition == null)
            index = 0;

        foreach (var canvas in _canvasList)
        {

            if (canvas.Position != null) continue;

            canvas.Position = index;
            index++;
        }

        _canvasList = convertedCanvasList.OrderBy(item => item.Position).ToList();
    }

    private void Draw(char?[,] matrix, bool skipEmpty = true, bool overwritePrevious = false)
    {
        for (var row = 0; row < matrix.GetLength(0); row++)
        {
            for (var column = 0; column < matrix.GetLength(1); column++)
            {
                var symbol = matrix[row, column];

                if ((symbol == null || symbol == char.Parse(" ")) && skipEmpty && !overwritePrevious)
                    continue;

                Write(column, row, symbol ?? char.Parse(" "));
            }
        }
    }

    private void DrawBorders()
    {
        foreach (var canvas in _canvasList)
        {
            if (canvas.ShowBorder)
            {
                canvas.Data[0, 0] = char.Parse(Borders.UlCorner);
                canvas.Data[0, canvas.Width] = char.Parse(Borders.UrCorner);
                canvas.Data[canvas.Height, canvas.Width] = char.Parse(Borders.LrCorner);
                canvas.Data[canvas.Height, 0] = char.Parse(Borders.LlCorner);

                for (int i = 1; i < canvas.Width; i++)
                {
                    canvas.Data[0, i] = char.Parse(Borders.Horizontal);
                    canvas.Data[canvas.Height, i] = char.Parse(Borders.Horizontal);
                }

                for (int i = 1; i < canvas.Height; i++)
                {
                    canvas.Data[i, 0] = char.Parse(Borders.Vertical);
                    canvas.Data[i, canvas.Width] = char.Parse(Borders.Vertical);
                }

                Draw(canvas.Data);
            }
        }
    }
}

internal class ConsoleCanvasConfigurationExtended : ConsoleCanvasConfiguration
{
    public ConsoleCanvasConfigurationExtended(ConsoleCanvasConfiguration configuration)
    {
        this.Name = configuration.Name;
        this.Height = configuration.Height;
        this.Position = configuration.Position;
        this.ShowBorder = configuration.ShowBorder;
        this.DefaultCursorPosition = configuration.DefaultCursorPosition;
    }

    public int HorizontalOffset { get; set; }
    public int VerticalOffset { get; set; }
    public int Width { get; set; }
    public char?[,] Data { get; set; } // can be used to verify what is drawn or should be drawn and then redraw only changes
}

internal class ConsoleCanvasConfiguration
{
    /// <summary>
    /// Canvas name
    /// </summary>
    public string? Name { get; set; } = null;

    /// <summary>
    /// Height in symbols
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Canvas position 0 stands for first or top most canvas
    /// </summary>
    public int? Position { get; set; }

    /// <summary>
    /// Enables borders for canvas
    /// </summary>
    public bool ShowBorder { get; set; } = false;

    /// <summary>
    /// Not implemented yet
    /// </summary>
    public (int x, int y)? DefaultCursorPosition { get; set; }
}


// Can write a logic save current cursor position to reuse for drawing