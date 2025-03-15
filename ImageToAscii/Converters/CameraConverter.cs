namespace ImageToAscii.Converters;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using AForge.Video.DirectShow;
using Helpers;

public class CameraConverter: IDisposable
{
    private readonly FilterInfo _camera;
    private readonly string _cameraId;
    private readonly string _asciiVocab;
    private readonly int _width;
    private readonly int _height;
    private readonly bool _reverseVocab;
    private VideoCaptureDevice _videoSource;
    public event EventHandler<string[]> NewFrame;

    public CameraConverter(string cameraId, string vocab, int width, int height, bool reverseVocab = false)
    {
        _cameraId = cameraId;
        _asciiVocab = vocab;
        _width = width;
        _height = height;
        _reverseVocab = reverseVocab;
    }

    public static FilterInfoCollection GetAllConnectedCameras()
    {
        FilterInfoCollection videoSources = new FilterInfoCollection(FilterCategory.VideoInputDevice);

        return videoSources;
    }

    public void StartRecording()
    {
        _videoSource = new VideoCaptureDevice(_cameraId);

        try
        {
            if (_videoSource.VideoCapabilities.Length > 0)
            {
                var resolution = FindClosestResolution();

                //Set the highest resolution as active
                _videoSource.VideoResolution = _videoSource.VideoCapabilities[Convert.ToInt32(resolution.Split(';')[1])];
            }
        }
        catch { }

        _videoSource.NewFrame += videoSource_NewFrame;
        _videoSource.Start();
    }

    private void videoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
    {
        NewFrame?.Invoke(this, ConvertBitmap((Bitmap)eventArgs.Frame.Clone()));
    }

    public void Dispose()
    {
        if (_videoSource is { IsRunning: true })
        {
            _videoSource.SignalToStop();
            _videoSource = null;
        }
    }

    private string FindClosestResolution()
    {
        string resolution = "0;0";

        int lowestMismatch = 0;
        for (int i = 0; i < _videoSource.VideoCapabilities.Length; i++)
        {
            if (_videoSource.VideoCapabilities[i].FrameSize.Width > _width)
            {
                var mismatch = _videoSource.VideoCapabilities[i].FrameSize.Width - _width;

                if (lowestMismatch == 0)
                    lowestMismatch = mismatch;

                if (mismatch < lowestMismatch && mismatch > 0)
                {
                    lowestMismatch = mismatch;
                    resolution = _videoSource.VideoCapabilities[i].FrameSize.Width.ToString() + ";" + i.ToString();
                }
                
            }
        }

        return resolution;
    }

    private string GetHighestResolution()
    {
        string highestSolution = "0;0";

        //Search for the highest resolution
        for (int i = 0; i < _videoSource.VideoCapabilities.Length; i++)
        {
            if (_videoSource.VideoCapabilities[i].FrameSize.Width > Convert.ToInt32(highestSolution.Split(';')[0]))
                highestSolution = _videoSource.VideoCapabilities[i].FrameSize.Width.ToString() + ";" + i.ToString();
        }

        return highestSolution;
    }

    private string[] ConvertBitmap(Bitmap bitmap)
    {
        Bitmap newBitmap = bitmap;
        if (bitmap.Height > _width || bitmap.Width > _height)
        {
            newBitmap = ResizeImage(bitmap);
        }

        var asciiLength = _asciiVocab.Length - 1;
        var convertedVocab = _asciiVocab;
        if (_reverseVocab)
        {
            convertedVocab = _asciiVocab.Reverse();
        }
        string[] asciiPicture = new string[newBitmap.Height];

        for (int y = 0; y < newBitmap.Height; y++)
        {
            string horizontalLine = string.Empty;
            for (int x = 0; x < newBitmap.Width; x++)
            {
                Color pixel = newBitmap.GetPixel(x, y);

                var r = pixel.R;
                var g = pixel.G;
                var b = pixel.B;
                var a = pixel.A;

                var avg = (r + g + b) / 3;

                var convertedAvg = ConvertToNewRange(avg, 255, asciiLength);

                char symbol = convertedVocab[convertedAvg];
                horizontalLine += symbol;
            }

            asciiPicture[y] = horizontalLine;
        }

        return asciiPicture;
    }

    private Bitmap ResizeImage(Bitmap bitmap)
    {
        Image image = bitmap;

        var destRect = new Rectangle(0, 0, _width, _height);
        var destImage = new Bitmap(_width, _height);

        destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        using var graphics = Graphics.FromImage(destImage);

        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        using var wrapMode = new ImageAttributes();
        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

        return destImage;
    }

    private int ConvertToNewRange(int value, int maxValue, int newRangeTo)
    {
        int originalRangeMax = maxValue;

        int percentilePosition = (int)Math.Round((double)(value * 100) / originalRangeMax);

        int convertedValue = (int)Math.Round((double)(newRangeTo * percentilePosition) / 100);

        if (convertedValue < 0)
        {
            convertedValue = 0;
        }

        if (convertedValue > newRangeTo)
        {
            convertedValue = newRangeTo;
        }

        return convertedValue;
    }
}