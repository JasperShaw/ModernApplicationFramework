using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
using Size = System.Drawing.Size;

namespace ModernApplicationFramework.Core.Utilities
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
            var size = new System.Windows.Size(width, height);
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
    }
}