namespace ConsoleHandler.Models;

public class ConsoleConfiguration
{
    public IList<ConsoleCanvasConfiguration> CanvasList { get; set; } = new List<ConsoleCanvasConfiguration>();

    /// <summary>
    /// Width in symbols
    /// MAX 150
    /// </summary>
    public int Width { get; set; } = 120;
}