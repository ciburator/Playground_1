using ImageToAscii.Helpers;
using System.Linq;
using System.Threading;
using ImageToAscii.Models;
using ImageToAscii.Renderers;

namespace ImageToAscii
{
    using System;

    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var configuration = new Configuration();
            var mainRenderer = new ConsoleRenderer(configuration);
        }
    }
}
