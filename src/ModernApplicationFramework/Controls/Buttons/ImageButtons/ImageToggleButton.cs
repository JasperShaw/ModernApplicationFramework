using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls.Buttons.ImageButtons
{
    /// <inheritdoc />
    /// <summary>
    ///  A toggle button created from an image. The image can have four states
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.Primitives.ToggleButton" />
    public class ImageToggleButton : ToggleButton
    {
        /// <summary>
        /// The hover image property
        /// </summary>
        public static readonly DependencyProperty HoverImageProperty = DependencyProperty.Register(
            "HoverImage", typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(default(ImageSource)));

        /// <summary>
        /// The normal image property
        /// </summary>
        public static readonly DependencyProperty NormalImageProperty = DependencyProperty.Register(
            "NormalImage", typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(default(ImageSource)));

        /// <summary>
        /// The pressed image property
        /// </summary>
        public static readonly DependencyProperty PressedImageProperty = DependencyProperty.Register(
            "PressedImage", typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(default(ImageSource)));

        /// <summary>
        /// The checked image property
        /// </summary>
        public static readonly DependencyProperty CheckedImageProperty = DependencyProperty.Register(
            "CheckedImage", typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(default(ImageSource)));


        static ImageToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageToggleButton),
                new FrameworkPropertyMetadata(typeof(ImageToggleButton)));
        }

        public ImageSource CheckedImage
        {
            get => (ImageSource) GetValue(CheckedImageProperty);
            set => SetValue(CheckedImageProperty, value);
        }

        public ImageSource HoverImage
        {
            get => (ImageSource) GetValue(HoverImageProperty);
            set => SetValue(HoverImageProperty, value);
        }

        public ImageSource NormalImage
        {
            get => (ImageSource) GetValue(NormalImageProperty);
            set => SetValue(NormalImageProperty, value);
        }

        public ImageSource PressedImage
        {
            get => (ImageSource) GetValue(PressedImageProperty);
            set => SetValue(PressedImageProperty, value);
        }
    }
}