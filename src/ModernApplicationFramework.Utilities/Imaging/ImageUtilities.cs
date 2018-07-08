using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
using Size = System.Windows.Size;

namespace ModernApplicationFramework.Utilities.Imaging
{
    public static class ImageUtilities
    {
        //http://tomelke.wordpress.com/2012/02/04/wpf-get-image-from-embedded-resource-file-from-code/
        public static Image CreateImageFromBitmapResource(string path)
        {
            var image = new Image();
            var thisassembly = Assembly.GetExecutingAssembly();
            var imageStream = thisassembly.GetManifestResourceStream(thisassembly.FullName + "." + path);
            if (imageStream == null)
                return image;
            var bmp = BitmapFrame.Create(imageStream);
            image.Source = bmp;
            return image;
        }

        public static Bitmap BitmapFromBitmapSource(BitmapSource bitmapSource)
        {
            return ImageConverter.BitmapFromBitmapSource(bitmapSource);
        }


        public static Image IconImageFromFrameworkElement(FrameworkElement e)
        {
            return ImageFromFrameworkElement(e, 16, 16);
        }

        public static Image ImageFromFrameworkElement(FrameworkElement e, int width, int height)
        {
            var size = new Size(width, height);
            e.Measure(size);
            e.Arrange(new Rect(size));

            RenderTargetBitmap targetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            targetBitmap.Render(e);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(targetBitmap));

            var stream = new MemoryStream();
            encoder.Save(stream);
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = new MemoryStream(stream.ToArray());
            bmp.EndInit();

            var i = new Image { Source = bmp };
            RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.Fant);
            return i;
        }

        public static BitmapSource FrameworkElementToBitmapSource(FrameworkElement target)
        {
            return FrameworkElementToBitmapSource(target, 96,96);
        }

        public static BitmapSource FrameworkElementToBitmapSource(FrameworkElement element, double dpiX, double dpiY)
        {
            var rect = new Rect(0.0, 0.0, 16, 16);
            var size = new Size(16, 16);
            var renderTargetBitmap = new RenderTargetBitmap(16, 16, DpiHelper.Default.LogicalDpiX, DpiHelper.Default.LogicalDpiY, PixelFormats.Pbgra32);
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