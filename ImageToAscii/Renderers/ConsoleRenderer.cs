namespace ImageToAscii.Renderers;

using Processors;
using System;
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