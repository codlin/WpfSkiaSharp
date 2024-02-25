using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfSkiaSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly LayerManager _layerManger = new LayerManager();
        SKSurface _surface;
        SKBitmap _bitmap;

        SKPoint _startPoint;
        SKPoint _endPoint;

        public MainWindow()
        {
            InitializeComponent();
            skiaCanvas1.InvalidateMeasure();

            // 加载图片
            LoadImage("Resources/demo.jpg");
        }

        void OnCanvasViewPaintSurface1(object sender, SKPaintSurfaceEventArgs e)
        {
            // 获取 SKCanvas 对象
            SKCanvas skCanvas = e.Surface.Canvas;
            _layerManger.SetCanvas(skCanvas);
            _layerManger.SetSurface(e.Surface);

            if (_layerManger.Canvas != null)
            {
                resultTextBlock.Text = "\\__在构造函数中触发重绘，在重绘方法中已拿到Canvas";
                tipsTextBlock.Text = "在蓝色框内点击鼠标右键，这会创建一个蓝色矩形框";
            }

            // 获取宋体在字体集合中的下标
            var index = SKFontManager.Default.FontFamilies.ToList().IndexOf("宋体");
            // 创建宋体字形
            var songtiTypeface = SKFontManager.Default.GetFontStyles(index).CreateTypeface(0);

            // 创建 SKPaint 对象
            SKPaint skPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Blue,
                IsAntialias = true,
                Typeface = songtiTypeface,
            };

            skCanvas.DrawText("不用点了，这会引发非法内存写，造成程序退出!", 50, 100, skPaint);
            skCanvas.DrawText("看来缓存 Canvas 是行不通的!", 50, 120, skPaint);
        }

        void LoadImage(string filePath)
        {
            try
            {
                // 使用 SkiaSharp 的 SKBitmap 加载图片
                using (var stream = File.OpenRead(filePath))
                {
                    _bitmap = SKBitmap.Decode(stream);
                }
            } catch (Exception ex)
            {
                MessageBox.Show($"Failed to load image: {ex.Message}");
            }
        }



        void SkiaCanvas1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        private void SkiaCanvas1_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //var canvas = _layerManger.Surface.Canvas;
            //// 绘制蓝色矩形
            //canvas.DrawRect(100, 100, 200, 200, new SKPaint() { Color = SKColors.Blue });
        }

        private void SkiaCanvas2_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SKCanvas canvas = _surface.Canvas;
            // 绘制蓝色矩形
            canvas.DrawRect(100, 100, 200, 200, new SKPaint() { Color = SKColors.Blue });
        }

        private void SkiaDraw(object sender, RoutedEventArgs e)
        {
            //var writeableBitmap = CreateImage(23000, 23000);
            var writeableBitmap = CreateImage(1920, 1080);
            UpdateImage(writeableBitmap);
            Image.Source = writeableBitmap;
        }

        private WriteableBitmap CreateImage(int width, int height)
        {
            var writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, BitmapPalettes.Halftone256Transparent);
            return writeableBitmap;
        }

        private void UpdateImage(WriteableBitmap writeableBitmap)
        {
            int width = (int)writeableBitmap.Width,
                height = (int)writeableBitmap.Height;

            writeableBitmap.Lock();
            var skImageInfo = new SKImageInfo()
            {
                Width = width,
                Height = height,
                ColorType = SKColorType.Bgra8888,
                AlphaType = SKAlphaType.Premul,
                ColorSpace = SKColorSpace.CreateSrgb()
            };

            var _surface = SKSurface.Create(skImageInfo, writeableBitmap.BackBuffer);
            SKCanvas canvas = _surface.Canvas;
            canvas.Clear(SKColors.White);
            canvas.DrawText("SkiaSharp on Wpf!", 50, 200, new SKPaint() { IsAntialias = true, Color = new SKColor(0, 0, 0), TextSize = 100 });
            canvas.DrawText("https://blog.lindexi.com", new SKPoint(50, 500), new SKPaint(new SKFont(SKTypeface.FromFamilyName("微软雅黑")))
            {
                IsAntialias = true,
                Color = new SKColor(0, 0, 0),
                TextSize = 20
            });
            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            writeableBitmap.Unlock();
        }

        void OnCanvasViewPaintSurface3(object sender, SKPaintSurfaceEventArgs e)
        {
            // 获取 SKCanvas 对象
            SKCanvas canvas = e.Surface.Canvas;
            Console.WriteLine($"OnCanvasViewPaintSurface SaveCount: {canvas.SaveCount}");

            canvas.Clear();
            canvas.Scale(scaleFactor, scaleFactor, mousePosition.X, mousePosition.Y);

            // 绘制图片
            if (_bitmap != null)
            {
                Console.WriteLine($"1 SaveCount:{canvas.SaveCount}");

                canvas.DrawBitmap(_bitmap, 0, 0);
            }

            Console.WriteLine($"2 SaveCount:{canvas.SaveCount}");

            // 画直线
            canvas.DrawLine(_startPoint, _endPoint, new SKPaint() { Color = SKColors.Red, StrokeWidth = 2 });
        }

        void SkiaCanvas3_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(skiaCanvas3).ToSKPoint();
        }

        private void skiaCanvas3_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // 更新鼠标位置
            mousePosition = e.GetPosition(skiaCanvas3).ToSKPoint();

            // 转换为 SKPoint
            _endPoint = mousePosition;
            skiaCanvas3.InvalidateVisual();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            skiaCanvas3.InvalidateVisual();
        }

        float scaleFactor = 1.0f;
        SKPoint mousePosition;
        private void SkiaCanvas3_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            // 获取鼠标点击的位置
            mousePosition = e.GetPosition(skiaCanvas3).ToSKPoint();

            float delta = e.Delta > 0 ? 0.1f : -0.1f; // 缩放步长
            scaleFactor += delta;

            skiaCanvas3.InvalidateVisual();
        }
    }
}
