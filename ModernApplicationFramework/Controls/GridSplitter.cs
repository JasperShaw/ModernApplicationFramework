using System.Windows;

namespace ModernApplicationFramework.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// A custom styled <see cref="T:System.Windows.Controls.GridSplitter" />
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.GridSplitter" />
    public class GridSplitter : System.Windows.Controls.GridSplitter
    {
        static GridSplitter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridSplitter),
                new FrameworkPropertyMetadata(typeof(GridSplitter)));
        }
    }
}