using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls
{
    public class ImageButton : Button
    {
        public static readonly DependencyProperty HoverImageProperty = DependencyProperty.Register(
            "HoverImage", typeof (ImageSource), typeof (ImageButton), new PropertyMetadata(default(ImageSource)));

        public static readonly DependencyProperty NormalImageProperty = DependencyProperty.Register(
            "NormalImage", typeof (ImageSource), typeof (ImageButton), new PropertyMetadata(default(ImageSource)));

        public static readonly DependencyProperty PressedImageProperty = DependencyProperty.Register(
            "PressedImage", typeof (ImageSource), typeof (ImageButton), new PropertyMetadata(default(ImageSource)));

        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ImageButton),
                new FrameworkPropertyMetadata(typeof (ImageButton)));
        }

        public ImageSource HoverImage
        {
            get { return (ImageSource) GetValue(HoverImageProperty); }
            set { SetValue(HoverImageProperty, value); }
        }

        public ImageSource NormalImage
        {
            get { return (ImageSource) GetValue(NormalImageProperty); }
            set { SetValue(NormalImageProperty, value); }
        }

        public ImageSource PressedImage
        {
            get { return (ImageSource) GetValue(PressedImageProperty); }
            set { SetValue(PressedImageProperty, value); }
        }
    }
}