namespace ImageToAscii.Renderers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using ConsoleHandler;
using ConsoleHandler.Models;

using Converters;

using Helpers;

using Models;

public class ConsoleRendererV2
{
    private readonly RenderMode _mode;
    private readonly string[] _imageNames;
    private readonly ConsoleHandler _handler;
    private readonly ImageConverter _imageConverter;

    public ConsoleRendererV2(RenderMode mode, string[] imageNames)
    {
        _mode = mode;
        _imageNames = imageNames;

        var handlerConfiguration = new ConsoleConfiguration
        {
            Width = MainConfiguration.Width,
            CanvasList = new List<ConsoleCanvasConfiguration>
            {
                new ConsoleCanvasConfiguration
                {
                    Name = "Header",
                    Height = 6,
                    Position = 0,
                    ShowBorder = false
                },
                new ConsoleCanvasConfiguration
                {
                    Name = "Picture",
                    Height = MainConfiguration.Height,
                    Position = 1,
                    ShowBorder = true
                }
            }
        };

        _handler = new ConsoleHandler(handlerConfiguration);

        _imageConverter = new ImageConverter(
            MainConfiguration.AsciiVocab,
            MainConfiguration.Width,
            MainConfiguration.Height);
    }

    public void Start()
    {
        UpdateHeader();

        SelectImageProcessor(_imageNames);
    }

    private void SelectImageProcessor(string[] imageNames)
    {
        switch (_mode)
        {
            case RenderMode.Image:
                SinglePictureDisplay(imageNames.First());
                break;
            case RenderMode.ImageContinuous:
                ContinuousPictureDisplay(imageNames);
                break;
            case RenderMode.Stream:
                StreamVideoDisplay();
                break;
            default:
                throw new Exception("Unknown mode selected");
        }
    }

    private void UpdateHeader()
    {
        _handler.Clear(0);

        _handler.WriteLine(0, $"Selected mode is: {Enum.GetName(_mode)}", null, "Header");

        var imagesText = "Selected ";

        if (_imageNames.Length > 1)
        {
            imagesText += " images: " + string.Join(",", _imageNames);
        }
        else
        {
            imagesText += $" image: {_imageNames.First()}";
        }

        _handler.WriteLine(1, imagesText, null, "Header");
    }

    private void SinglePictureDisplay(string imageName, bool clear = true)
    {
        if (clear)
            _handler.Clear(1);

        var imageUrl = FileHelper.GetImageUrl(imageName);
        var stringImage = _imageConverter.GetStringImageMatrix(
            imageUrl, MainConfiguration.ReverseVocab);

        for (int y = 0; y < stringImage.Length; y++)
        {
            _handler.WriteLine(y, stringImage[y], 1);

            //for (int x = 0; x < stringImage[y].Length; x++)
            //{
            //    char currentPixel = stringImage[y][x];
            //    _handler.Write(x,y,currentPixel,1,"Picture");
            //}
        }
    }

    private void ContinuousPictureDisplay(string[] images)
    {
        var timer = new Stopwatch();
        while (true)
        {
            foreach (var image in images)
            {
                timer.Reset();
                timer.Start();

                SinglePictureDisplay(image, false);

                timer.Stop();

                Debug.WriteLine($"Drawing {image} took time to process {timer.ElapsedMilliseconds}");

                //Thread.Sleep(100);
            }
        }
    }

    private void StreamVideoDisplay()
    {
        throw new NotImplementedException("Stream video display is not yet implemented");
    }
}