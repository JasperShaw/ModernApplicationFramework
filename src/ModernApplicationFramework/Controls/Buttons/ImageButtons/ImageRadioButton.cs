using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls.Buttons.ImageButtons
{
    /// <inheritdoc />
    /// <summary>
    /// A radio button created from an image. The image can have four states
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.RadioButton" />
    public class ImageRadioButton : RadioButton
    {
        /// <summary>
        /// The hover image property
        /// </summary>
        public static readonly DependencyProperty HoverImageProperty = DependencyProperty.Register(
            "HoverImage", typeof(ImageSource), typeof(ImageRadioButton), new PropertyMetadata(default(ImageSource)));

        /// <summary>
        /// The normal image property
        /// </summary>
        public static readonly DependencyProperty NormalImageProperty = DependencyProperty.Register(
            "NormalImage", typeof(ImageSource), typeof(ImageRadioButton), new PropertyMetadata(default(ImageSource)));

        /// <summary>
        /// The pressed image property
        /// </summary>
        public static readonly DependencyProperty PressedImageProperty = DependencyProperty.Register(
            "PressedImage", typeof(ImageSource), typeof(ImageRadioButton), new PropertyMetadata(default(ImageSource)));

        /// <summary>
        /// The checked image property
        /// </summary>
        public static readonly DependencyProperty CheckedImageProperty = DependencyProperty.Register(
            "CheckedImage", typeof(ImageSource), typeof(ImageRadioButton), new PropertyMetadata(default(ImageSource)));


        static ImageRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageRadioButton),
                new FrameworkPropertyMetadata(typeof(ImageRadioButton)));
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