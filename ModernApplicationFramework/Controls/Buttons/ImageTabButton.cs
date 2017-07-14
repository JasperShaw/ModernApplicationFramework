using System.Windows;

namespace ModernApplicationFramework.Controls.Buttons
{
    public class ImageTabButton : ImageRadioButton
    {
        static ImageTabButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageTabButton),
                new FrameworkPropertyMetadata(typeof(ImageTabButton)));
        }
    }
}