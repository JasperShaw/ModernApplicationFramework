using System.Windows;

namespace ModernApplicationFramework.Controls
{
    /// <summary>
    /// A custom styled <see cref="System.Windows.Controls.ScrollViewer"/> control
    /// </summary>
    /// <seealso cref="System.Windows.Controls.ScrollViewer" />
    public class ScrollViewer : System.Windows.Controls.ScrollViewer
    {
        static ScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollViewer), new FrameworkPropertyMetadata(typeof(ScrollViewer)));
        }
    }
}
