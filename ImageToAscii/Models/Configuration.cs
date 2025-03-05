namespace ImageToAscii.Models;

using System.Collections.Generic;
using Renderers;

public class Configuration
{
    public string AsciiVocab { get; set; } = "Ñ@#W$9876543210?!abc;:+=-,._ ";
    public bool ReverseVocab { get; set; } = false;

    /// <summary>
    /// MAX 150
    /// </summary>
    public int Width { get; set; } = 120;

    /// <summary>
    /// MAX 100
    /// </summary>
    public int Height { get; set; } = 60;

    public int WidthOffset { get; set; } = 3;
    public int HeightOffset { get; set; } = 3;

    public int ImageXOffset { get; set; } = 0;
    public int ImageYOffset { get; set; } = 0;

    public string ImageName { get; set; } = "hooligan-main.png";

    public RenderMode Mode { get; set; } = RenderMode.Image;
    public int CycleDelay { get; set; } = 2000; // milliseconds
    public bool EnableBorders { get; set; } = true;
    public int OffsetFromTop { get; set; } = 0;
}

public static class MainConfiguration
{
    public const string AsciiVocab = "Ñ@#W$9876543210?!abc;:+=-,._ ";
    public const bool ReverseVocab = false;

    /// <s constummary>
    /// MA constX 150
    /// </ constsummary>
    public const int Width = 120;

    /// <s constummary>
    /// MA constX 100
    /// </ constsummary>
    public const int Height = 60;

    public const string ImageName = "hooligan-main.jpg";

    public const RenderMode Mode = RenderMode.Image;
    public const int CycleDelay = 2000; // milliseconds
}
