namespace ImageToAscii.Renderer;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AForge.Video.DirectShow;
using ConsoleHandler;
using ConsoleHandler.Models;

using Converters;

using Helpers;

using Models;

public class ConsoleRendererV2
{
    private readonly RenderMode _mode;
    private readonly string[] _imageNames;
    private readonly FilterInfo? _camera;
    private readonly ConsoleHandler _handler;

    private ImageConverter? _imageConverter;
    private CameraConverter? _cameraConverter;

    public ConsoleRendererV2(RenderMode mode, string[] imageNames, FilterInfo? camera)
    {
        _mode = mode;
        _imageNames = imageNames;
        _camera = camera;

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
        _imageConverter = new ImageConverter(
            MainConfiguration.AsciiVocab,
            MainConfiguration.Width,
            MainConfiguration.Height);

        if (clear)
            _handler.Clear(1);

        var imageUrl = FileHelper.GetImageUrl(imageName);
        var stringImage = _imageConverter.GetStringImageMatrix(
            imageUrl, MainConfiguration.ReverseVocab);

        for (int y = 0; y < stringImage.Length; y++)
        {
            _handler.WriteLine(y, stringImage[y], 1);
        }
    }

    private void ContinuousPictureDisplay(string[] images)
    {
        _imageConverter = new ImageConverter(
            MainConfiguration.AsciiVocab,
            MainConfiguration.Width,
            MainConfiguration.Height);

        var timer = new Stopwatch();

        IList<(string name, string[] data)> convertedImageList = new List<(string name, string[] data)>();
 
        foreach (var image in images)
        {
            var imageUrl = FileHelper.GetImageUrl(image);
            var stringImage = _imageConverter.GetStringImageMatrix(
                imageUrl, MainConfiguration.ReverseVocab);

            convertedImageList.Add((image, stringImage));
        }

        while (true)
        {
            foreach (var image in convertedImageList)
            {
                timer.Reset();
                timer.Start();

                for (int y = 0; y < image.data.Length; y++)
                {
                    _handler.WriteLine(y, image.data[y], 1);
                }

                timer.Stop();

                Debug.WriteLine($"Drawing {image.name} took time to process {timer.ElapsedMilliseconds}");

                Thread.Sleep(300);
            }
        }
    }

    private void StreamVideoDisplay()
    {
        if (_camera == null)
            throw new Exception("No camera selected");

        _cameraConverter = new CameraConverter(
            _camera, 
            MainConfiguration.AsciiVocab,
            MainConfiguration.Width,
            MainConfiguration.Height,
            MainConfiguration.ReverseVocab);

        _cameraConverter.NewFrame += _cameraConverter_NewFrame;

        _cameraConverter.StartRecording();
    }

    private void _cameraConverter_NewFrame(object sender, string[] image)
    {
        for (int y = 0; y < image.Length; y++)
        {
            _handler.WriteLine(y, image[y], 1);
        }
    }
}