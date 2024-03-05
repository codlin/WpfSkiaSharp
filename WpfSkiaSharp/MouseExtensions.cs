using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace WpfSkiaSharp
{
    public static class MouseExtensions
    {
        public static Point GetDpiPosition(this MouseEventArgs e, IInputElement relativeTo)
        {
            var mainWindow = Application.Current.MainWindow;
            var dpiScale = VisualTreeHelper.GetDpi(mainWindow);

            var dpiScaleX = dpiScale.DpiScaleX;
            var dpiScaleY = dpiScale.DpiScaleY;

            var pixelPosition = e.GetPosition(relativeTo);
            return new Point(pixelPosition.X * dpiScaleX, pixelPosition.Y * dpiScaleY);
        }
    }
}
