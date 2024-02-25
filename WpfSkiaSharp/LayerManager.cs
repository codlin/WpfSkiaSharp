using SkiaSharp;

namespace WpfSkiaSharp
{
    internal class LayerManager
    {
        public SKCanvas Canvas { get; private set; }
        public SKSurface Surface { get; private set; }

        public void SetCanvas(SKCanvas canvas)
        {
            Canvas = canvas;
        }
        public void SetSurface(SKSurface surface) { Surface = surface; }
    }
}
