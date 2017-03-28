using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Test;
using Image = System.Windows.Controls.Image;
using Size = System.Windows.Size;

namespace ModernApplicationFramework.Controls
{
    public class CommandDefinitionButton : System.Windows.Controls.Button
    {
        public CommandDefinitionButton(ToolbarItemDefinition definition)
        {
            DataContext = definition;
            SetIcon();
        }

        public void SetIcon()
        {
            var def = DataContext as ToolbarItemDefinition;
            if (def == null)
                return;
            object vb = null;
            if (!string.IsNullOrEmpty(def.CommandDefinition.IconSource?.OriginalString))
            {
                var myResourceDictionary = new ResourceDictionary { Source = def.CommandDefinition.IconSource };
                vb = myResourceDictionary[def.CommandDefinition.IconId];
            }
            var vbr = vb as Viewbox;
            var i = ImageFromFrameworkElement(vbr);
            RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.Fant);

            var b = ImageUtilities.BitmapFromBitmapSource((BitmapSource)i.Source);
            var bi = ImageThemingUtilities.GetThemedBitmap(b, ImageThemingUtilities.GetImageBackgroundColor(this).ToRgba());
            var bs = ImageConverter.BitmapSourceFromBitmap(bi);
            i.Source = bs;
            Icon = i;
        }

        


        public static Image ImageFromFrameworkElement(FrameworkElement e)
        {
            var size = new Size(16, 16);
            e.Measure(size);
            e.Arrange(new Rect(size));

            RenderTargetBitmap targetBitmap = new RenderTargetBitmap(16, 16, 96, 96, PixelFormats.Pbgra32);
            targetBitmap.Render(e);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(targetBitmap));

            var stream = new MemoryStream();
            encoder.Save(stream);
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = new MemoryStream(stream.ToArray());
            bmp.EndInit();

            var i = new Image {Source = bmp};
            RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.Fant);
            return i;
        }

        public object Icon { get; set; }
    }
}
