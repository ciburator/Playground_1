namespace ImageToAscii.Models;

using Renderers;
using System.Collections.Generic;
using Handlers;

internal class ConsoleConfiguration
{
    public IList<ConsoleCanvasConfiguration> CanvasList { get; set; }

    /// <summary>
    /// Width in symbols
    /// MAX 150
    /// </summary>
    public int Width { get; set; } = 120;

    /// <summary>
    /// Described by symbols in order
    /// TL - Top Left
    /// BR - Bottom Right
    /// H - Horizontal
    /// V - Vertical
    /// TL, TR, BL, BR, H, V
    /// </summary>
    public string Borders { get; set; } = "╔╗╚╝═║";
}
