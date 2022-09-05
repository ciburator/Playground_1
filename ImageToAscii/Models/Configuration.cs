namespace ImageToAscii.Models
{
    public class Configuration
    {
        public string AsciiVocab { get; set; } = "Ñ@#W$9876543210?!abc;:+=-,._ ";
        public bool ReverseVocab { get; set; } = false;

        public int Width { get; set; } = 200;
        public int Height { get; set; } = 80;

        public int WidthOffset { get; set; } = 3;
        public int HeightOffset { get; set; } = 3;

        public int ImageXOffset { get; set; } = 0;
        public int ImageYOffset { get; set; } = 0;

        public string ImageName { get; set; } = "ciblogo3.png";

        public RenderMode Mode { get; set; } = RenderMode.ImageContinuous;
        public int CycleDelay { get; set; } = 2000; // milliseconds
        public bool EnableBorders { get; set; } = true;
    }
}