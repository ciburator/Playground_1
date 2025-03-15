namespace ConsoleHandler;

using Models;

public class ConsoleHandler
{
    private const int DefaultWindowHorizontalBorderOffset = 3;
    private const int DefaultWindowVerticalBorderOffset = 3;

    private const int CanvasVerticalOffset = 1;

    public bool IsInitialized { get; private set; }
    private readonly int _height;
    private readonly int _width;

    private ConsoleCanvasConfigurationExtended? _lastCanvas;

    private IList<ConsoleCanvasConfigurationExtended> _canvasList = new List<ConsoleCanvasConfigurationExtended>();

    public ConsoleHandler(ConsoleConfiguration config)
    {
        if (config.CanvasList.Count == 0)
            throw new Exception("No canvas provided");

        var isBordersEnabled = config.CanvasList.FirstOrDefault(item => item.ShowBorder) != null;
        _width = isBordersEnabled ? config.Width + 2 : config.Width;

        PrepareCanvasList(config.CanvasList);
        OrderCanvasList();

        _height = _canvasList.Select(item => item.Height).Sum();

        Console.SetWindowSize(
            _width + DefaultWindowHorizontalBorderOffset,
            _height + DefaultWindowVerticalBorderOffset);

        foreach (var canvas in _canvasList)
        {
            DrawMatrix(canvas);
        }

        IsInitialized = true;
    }

    public void Clear(int? canvasPosition = null, string? canvasName = null)
    {
        var canvas = GetCanvas(canvasPosition, canvasName);

        for (int y = 0; y < canvas.ImageHeight; y++)
        {
            var line = string.Empty;
            for (int x = 0; x < canvas.ImageWidth; x++)
            {
                var posX = x + canvas.ImageHorizontalOffset;
                var posY = y + canvas.ImageVerticalOffset;
                canvas.Data[posY, posX] = ' ';

                line += ' ';
            }

            WriteLine(y, line, canvas);
        }
    }

    public void WriteLine(int row, string text, int? canvasPosition = null, string? canvasName = null)
    {
        var canvas = GetCanvas(canvasPosition, canvasName);

        var posX = canvas.ImageHorizontalOffset;
        var posY = row + canvas.ImageVerticalOffset;

        int x = posX;
        foreach (char letter in text)
        {
            canvas.Data[posY, x] = letter;
            x++;
        }

        WriteLine(posX+canvas.HorizontalOffset, posY + canvas.VerticalOffset, text);
    }

    public void Write(int x, int y, char symbol, int? canvasPosition = null, string? canvasName = null)
    {
        var canvas = GetCanvas(canvasPosition, canvasName);

        var posX = x + canvas.ImageHorizontalOffset;
        var posY = y + canvas.ImageVerticalOffset;

        if (canvas.Data[posY, posX] == symbol)
            return;

        canvas.Data[posY, posX] = symbol;

        Write(posX, posY, symbol, canvas);
    }

    private void Write(int x, int y, char symbol, ConsoleCanvasConfigurationExtended canvas)
    {
        Write(x + canvas.HorizontalOffset, y + canvas.VerticalOffset, symbol);
    }

    private void WriteLine(int y, string text, ConsoleCanvasConfigurationExtended canvas)
    {
        var posX = canvas.ImageHorizontalOffset;
        var posY = y + canvas.ImageVerticalOffset;

        WriteLine(posX + canvas.HorizontalOffset, posY + canvas.VerticalOffset, text);
    }

    private void DrawMatrix(ConsoleCanvasConfigurationExtended canvas, bool skipEmpty = true, bool overwritePrevious = false)
    {
        char?[,] matrix = canvas.Data;

        for (var row = 0; row < matrix.GetLength(0); row++)
        {
            for (var column = 0; column < matrix.GetLength(1); column++)
            {
                var symbol = matrix[row, column];

                if ((symbol == null || symbol == char.Parse(" ")) && skipEmpty && !overwritePrevious)
                    continue;

                var posX = column;
                var posY = row + canvas.VerticalOffset;

                Write(posX, posY, symbol ?? char.Parse(" "));
            }
        }
    }

    #region Console window manipulation
    private void Write(int x, int y, char symbol)
    {
        Console.SetCursorPosition(x, y);
        Console.Write(symbol);
    }

    private void WriteLine(int x, int y, string text)
    {
        Console.SetCursorPosition(x, y);
        Console.WriteLine(text);
    }
    #endregion

    #region Canvas list operations
    /// <summary>
    /// Recreates canvas in extended format for internal handling/processing
    /// </summary>
    /// <param name="canvasList"></param>
    private void PrepareCanvasList(IList<ConsoleCanvasConfiguration> canvasList)
    {

        foreach (var canvas in canvasList)
        {
            var verticalOffset = canvasList
                .Where(item => item.Position < canvas.Position)
                .Sum(item => item.Height);

            var fullVerticalOffset = verticalOffset == 0 ? verticalOffset : verticalOffset + CanvasVerticalOffset;

            var extendedCanvas = new ConsoleCanvasConfigurationExtended(canvas, fullVerticalOffset, _width);

            _canvasList.Add(extendedCanvas);
        }
    }

    /// <summary>
    /// Reorders list by provided position or automatically arranges
    /// </summary>
    private void OrderCanvasList()
    {
        if (_canvasList.Count <= 1) return;

        _canvasList = _canvasList.OrderBy(item => item.Position).ToList();

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

        _canvasList = _canvasList.OrderBy(item => item.Position).ToList();
    }

    /// <summary>
    /// Retrieves canvas from list by provided parameters
    /// </summary>
    /// <param name="canvasPosition">canvas position</param>
    /// <param name="canvasName">canvas name</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private ConsoleCanvasConfigurationExtended GetCanvas(int? canvasPosition = null, string? canvasName = null)
    {
        ConsoleCanvasConfigurationExtended? result = null;

        if ( _lastCanvas != null && (_lastCanvas.Position == canvasPosition || _lastCanvas?.Name == canvasName))
            return _lastCanvas;

        if (canvasPosition != null)
            result = GetCanvasByPosition((int)canvasPosition);

        if (!string.IsNullOrWhiteSpace(canvasName))
            result = GetCanvasByName(canvasName);

        if (_canvasList.Count == 1)
            result = _canvasList[0];

        if (result != null)
        {
            _lastCanvas = result;
            return result;
        }

        throw new Exception("Please provide at least one identifier");
    }

    /// <summary>
    /// Retrieves canvas by name
    /// </summary>
    /// <param name="canvasName"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private ConsoleCanvasConfigurationExtended GetCanvasByName(string canvasName)
    {
        if (string.IsNullOrWhiteSpace(canvasName))
            throw new Exception("Please provide name");

        return _canvasList.First(item => String.Equals(item.Name, canvasName, StringComparison.CurrentCultureIgnoreCase));
    }

    /// <summary>
    /// Retrieves canvas by position
    /// </summary>
    /// <param name="canvasPosition"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private ConsoleCanvasConfigurationExtended GetCanvasByPosition(int canvasPosition)
    {
        if (canvasPosition >= _canvasList.Count)
            throw new Exception("Out of bounds position");

        return _canvasList.First(item => item.Position == canvasPosition);
    }
    #endregion
}
