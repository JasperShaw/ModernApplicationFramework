using System.Windows;

namespace ModernApplicationFramework.Controls.Buttons
{
    /// <inheritdoc />
    /// <summary>
    /// A restyled button control
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.Button" />
    public class Button : System.Windows.Controls.Button
    {
        static Button()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(typeof(Button)));
        }
    }
}