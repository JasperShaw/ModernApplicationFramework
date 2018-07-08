using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Basics.Imaging
{
    public sealed class CrispImage : Image
    {
        public static readonly DependencyProperty MonikerProperty =
            DependencyProperty.Register(nameof(Moniker), typeof(ImageMoniker), typeof(CrispImage));

        public ImageMoniker Moniker
        {
            get => (ImageMoniker) GetValue(MonikerProperty);
            set
            {
                SetValue(MonikerProperty, value);
                Source = ImageLibrary.Instance.GetImage(value);
            }
        }

        static CrispImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CrispImage), new FrameworkPropertyMetadata(typeof(CrispImage)));
        }
    }
}
