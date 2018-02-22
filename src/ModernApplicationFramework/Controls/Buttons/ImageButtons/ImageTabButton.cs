using System.Windows;

namespace ModernApplicationFramework.Controls.Buttons.ImageButtons
{
    /// <inheritdoc />
    /// <summary>
    /// A tab button created from an image
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Controls.Buttons.ImageRadioButton" />
    public class ImageTabButton : ImageRadioButton
    {
        static ImageTabButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageTabButton),
                new FrameworkPropertyMetadata(typeof(ImageTabButton)));
        }
    }
}