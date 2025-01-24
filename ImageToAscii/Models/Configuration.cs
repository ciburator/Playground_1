namespace ImageToAscii.Models;

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

    public string ImageName { get; set; } = "ciblogo3.png";

    public RenderMode Mode { get; set; } = RenderMode.Image;
    public int CycleDelay { get; set; } = 2000; // milliseconds
    public bool EnableBorders { get; set; } = true;
    public int OffsetFromTop { get; set; } = 0;
}