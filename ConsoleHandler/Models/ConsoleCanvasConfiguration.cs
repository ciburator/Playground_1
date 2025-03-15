namespace ConsoleHandler.Models;

public class ConsoleCanvasConfiguration
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