namespace ImageToAscii.Models;

public static class MainConfiguration
{
    public const string AsciiVocab = "Ñ@#W$9876543210?!abc;:*+=-,._ ";
    public const bool ReverseVocab = true;

    /// <summary>
    /// MA constX 150
    /// </summary>
    public const int Width = 200;

    /// <summary>
    /// MA constX 100
    /// </summary>
    public const int Height = 70;

    public const string ImageName = "hooligan-main.jpg";

    public const RenderMode Mode = RenderMode.Image;
    public const int CycleDelay = 2000; // milliseconds
}
