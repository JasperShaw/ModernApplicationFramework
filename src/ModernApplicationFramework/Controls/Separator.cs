using System.Windows;

namespace ModernApplicationFramework.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// A custom styled <see cref="T:System.Windows.Controls.Separator" /> control
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.Separator" />
    public class Separator : System.Windows.Controls.Separator
    {
        static Separator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Separator), new FrameworkPropertyMetadata(typeof(Separator)));
        }
    }
}