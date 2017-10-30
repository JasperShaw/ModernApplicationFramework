using System.Windows;

namespace ModernApplicationFramework.Controls.Primitives
{
    /// <inheritdoc />
    /// <summary>
    /// A custom styled <see cref="T:System.Windows.Controls.Primitives.ScrollBar" />
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.Primitives.ScrollBar" />
    public class ScrollBar : System.Windows.Controls.Primitives.ScrollBar
    {
        static ScrollBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollBar), new FrameworkPropertyMetadata(typeof(ScrollBar)));
        }
    }
}