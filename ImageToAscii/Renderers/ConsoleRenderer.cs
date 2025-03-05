namespace ImageToAscii.Renderers;

using Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using Models;

public class ConsoleRenderer
{
    private readonly Configuration config;
    private BaseProcessor _processor;

    public ConsoleRenderer(Configuration config)
    {
        this.config = config;

        this.PrepareConsole();

        Write($"Showing Image: {config.ImageName}");

        this.SelectedRenderer();

        this.BeginDraw();
    }

    private void BeginDraw()
    {
        this._processor.Init();
    }

    private void SelectedRenderer()
    {
        switch (this.config.Mode)
        {
            case RenderMode.Image:
                this._processor = new ImageProcessor(config);
                break;
            case RenderMode.ImageContinuous:
                this._processor = new ImageProcessor(config, true);
                break;
            default:
                break;
        }
    }

    private void PrepareConsole()
    {
        Console.SetWindowSize(
            this.config.Width + this.config.WidthOffset,
            this.config.Height + this.config.HeightOffset+1);
    }

    private void Write(string text)
    {
        if (text.Length > (this.config.Width + this.config.WidthOffset))
        {
            var chunkSize = (this.config.Width + this.config.WidthOffset);
            var list = Enumerable.Range(0, text.Length / chunkSize)
                .Select(i => text.Substring(i * chunkSize, chunkSize));

            foreach (var item in list)
            {
                Console.WriteLine(item);
                this.config.OffsetFromTop += 1;
            }
        }
        else
        {
            Console.WriteLine(text);
            this.config.OffsetFromTop += 1;
        }
    }
}

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

    private IList<ConsoleCanvasConfiguration> _canvasList;

    public ConsoleHandler(ConsoleConfiguration config)
    {
        if (config.CanvasList.Count == 0)
            throw new Exception("No canvas provided");

        OrderCanvasList(config.CanvasList);
        
        _width = config.Width;
        _height = _canvasList.Select(item => item.Height).Sum();

        Console.SetWindowSize(
            _width + DefaultWindowHorizontalBorderOffset,
            _height + DefaultWindowVerticalBorderOffset);

        IsInitialized = true;
    }

    public void Clear(int? canvasPosition = null, string? canvasName = null)
    {
        Check();
        throw new NotImplementedException();

        var canvas = GetCanvas(canvasPosition, canvasName);
        var offset = GetCanvasOffset(canvasPosition, canvasName);

        for (int y = 0; y <canvas.Height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Write();
            }
        }

        

    }

    public void WriteLine(string text, int? canvasPosition = null, string? canvasName = null)
    {
        Check();
        var offset = GetCanvasOffset(canvasPosition, canvasName);
        Console.SetCursorPosition(offset.xOffset, offset.yOffset);
        Console.WriteLine(text);
    }

    public void Write(int x, int y, char symbol, int? canvasPosition = null, string? canvasName = null)
    {
        Check();

        var offset = GetCanvasOffset(canvasPosition, canvasName);

        Console.SetCursorPosition(x + offset.xOffset, y + offset.yOffset);
        Console.Write(symbol);
    }

    public ConsoleCanvasConfiguration GetCanvas(int? canvasPosition = null, string? canvasName = null)
    {
        if (canvasPosition != null)
            return GetCanvas(canvasPosition);

        if(!string.IsNullOrWhiteSpace(canvasName))
            return GetCanvas(canvasName);

        if(_canvasList.Count == 1)
            return _canvasList[0];

        throw new Exception("Please provide at least one identifier");
    }

    public ConsoleCanvasConfiguration GetCanvas(string canvasName)
    {
        if (string.IsNullOrWhiteSpace(canvasName))
            throw new Exception("Please provide name");

        return _canvasList.First(item => String.Equals(item.Name, canvasName, StringComparison.CurrentCultureIgnoreCase));
    }

    public ConsoleCanvasConfiguration GetCanvas(int canvasPosition)
    {
        if (canvasPosition >= _canvasList.Count)
            throw new Exception("Out of bounds position");

        return _canvasList.First(item => item.Position == canvasPosition);
    }

    private (int xOffset, int yOffset, int width, int height) GetCanvasOffset(int? position, string? canvasName)
    {
        if (position == null && string.IsNullOrWhiteSpace(canvasName) && _canvasList.Count > 1)
            throw new Exception("Cant choose canvas to edit");
        else if(_canvasList.Count == 1)
            return (DefaultBorderHorizontalOffset, DefaultBorderVerticalOffset, _width, _canvasList.First().Height);

        if (position != null)
        {
            if ((position + 1) < _canvasList.Count)
                throw new Exception("Out of bounds position selected");

            var selectedCanvas = _canvasList.First(item => item.Position == position);

            //Add 1 to account for position starting from 0
            var offsetMultiplier = (int)(selectedCanvas.Position! + 1);

            var verticalOffset = _canvasList
                .Where(item => item.Position < selectedCanvas.Position)
                .Sum(item => item.Height);

            return (DefaultBorderHorizontalOffset, DefaultBorderVerticalOffset * offsetMultiplier + verticalOffset, _width, selectedCanvas.Height);
        }

        if (!string.IsNullOrWhiteSpace(canvasName))
        {
            var selectedCanvas = _canvasList.FirstOrDefault(item => item.Name == canvasName);

            if (selectedCanvas == null)
                throw new Exception("Canvas by defined name is not found");

            //Add 1 to account for position starting from 0
            var offsetMultiplier = (int)(selectedCanvas.Position! + 1);

            var verticalOffset = _canvasList
                .Where(item => item.Position < selectedCanvas.Position)
                .Sum(item => item.Height);

            return (DefaultBorderHorizontalOffset, DefaultBorderVerticalOffset * offsetMultiplier + verticalOffset, _width, selectedCanvas.Height);
        }

        throw new Exception("GetCanvasOffset exception");
    }

    private void Check()
    {
        if (!IsInitialized)
            throw new Exception("Please initialize first");
    }

    private void OrderCanvasList(IList<ConsoleCanvasConfiguration> canvasList)
    {
        if (canvasList.Count <= 1) return;

        _canvasList = canvasList.OrderBy(item => item.Position).ToList();

        var maxPosition = _canvasList.Max(item => item.Position);

        if (maxPosition == null)
        {
            var index = 0;
            foreach (var canvas in _canvasList)
            {
                canvas.Position = index;
                index++;
            }
        }
        else if (maxPosition + 1 < _canvasList.Count)
        {
            var index = maxPosition + 1;
            foreach (var canvas in _canvasList)
            {
                if (canvas.Position != null) continue;

                canvas.Position = index;
                index++;
            }
        }
        else
        {
            return;
        }

        _canvasList = canvasList.OrderBy(item => item.Position).ToList();
    }
}

internal class ConsoleCanvasConfigurationExtended : ConsoleCanvasConfiguration
{
    public int HorizontalOffset { get; set; }
    public int VerticalOffset { get; set; }
    public char[,] Data { get; set; } // can be used to verify what is drawn or should be drawn and then redraw only changes
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