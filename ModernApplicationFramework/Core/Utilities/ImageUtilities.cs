using System.Drawing;
using System.Reflection;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

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
    }
}