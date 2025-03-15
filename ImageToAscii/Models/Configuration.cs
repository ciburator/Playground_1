namespace ImageToAscii.Models;

public static class MainConfiguration
{
    public const string AsciiVocab = "Ñ@#W$9876543210?!abc;:*+=-,._ ";
    public const bool ReverseVocab = true;
    public const int Width = 200;
    public const int Height = 70;
    public const string? ImageName = "hooligan-main.jpg";
    public const RenderMode Mode = RenderMode.Image;
    public const int CycleDelay = 100; // milliseconds
    public const int RenderDelay = 10;
}
