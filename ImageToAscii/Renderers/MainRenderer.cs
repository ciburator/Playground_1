using System;
using ImageToAscii.Models;

namespace ImageToAscii.Renderers
{
    public class MainRenderer
    {
        private readonly Configuration config;
        private BaseRenderer renderer;

        public MainRenderer(Configuration config)
        {
            this.config = config;

            this.PrepareConsole();

            this.SelectedRenderer();

            this.BeginDraw();
        }

        private void BeginDraw()
        {
            this.renderer.Init();
        }

        private void SelectedRenderer()
        {
            switch (this.config.Mode)
            {
                case RenderMode.Image:
                    this.renderer = new PictureRenderer(config);
                    break;
                case RenderMode.ImageContinuous:
                    this.renderer = new PictureRenderer(config, true);
                    break;
                default:
                    break;
            }
        }

        private void PrepareConsole()
        {
            Console.SetWindowSize(
                this.config.Width + this.config.WidthOffset,
                this.config.Height + this.config.HeightOffset);
        }
    }
}