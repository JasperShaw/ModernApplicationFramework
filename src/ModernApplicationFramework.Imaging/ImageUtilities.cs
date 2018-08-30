using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Utilities;
using Size = System.Windows.Size;

namespace ModernApplicationFramework.Imaging
{
    public static class ImageUtilities
    {
        public static BitmapSource FrameworkElementToBitmapSource(FrameworkElement element, Size size)
        {
            var rect = new Rect(0.0, 0.0, size.Width, size.Height);
            var renderTargetBitmap = new RenderTargetBitmap((int) size.Width, (int) size.Height,
                DpiHelper.Default.LogicalDpiX, DpiHelper.Default.LogicalDpiY, PixelFormats.Pbgra32);
            element.Width = rect.Width;
            element.Height = rect.Height;
            element.Measure(size);
            element.Arrange(rect);
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                var visualBrush = new VisualBrush( element) {Stretch = Stretch.Uniform};
                drawingContext.DrawRectangle(visualBrush, null, rect);
            }
            renderTargetBitmap.Render(drawingVisual);
            return new FormatConvertedBitmap(renderTargetBitmap, PixelFormats.Bgra32, null, 0.0);
        }
    }
}