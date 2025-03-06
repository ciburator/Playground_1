namespace ImageToAscii.Interfaces;

using Handlers;
using ImageToAscii.Models;
using Renderers;

internal interface IRendererHandler
{
    bool IsInitialized { get; }

    void Clear();

    void WriteLine(string text);

    void Write(int x, int y, char symbol);

    ConsoleCanvasConfiguration GetCanvas(int? position);
}