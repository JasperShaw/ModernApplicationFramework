using System.Windows;

namespace ModernApplicationFramework.Controls.Buttons
{
    /// <inheritdoc />
    /// <summary>
    /// A button control special designed for dialog windows
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.Button" />
    public class DialogButton: System.Windows.Controls.Button
    {
        static DialogButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogButton), new FrameworkPropertyMetadata(typeof(DialogButton)));
        }
    }
}
