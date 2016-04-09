using System.Windows;

namespace ModernApplicationFramework.Controls
{
    public class GridSplitter : System.Windows.Controls.GridSplitter
    {
        static GridSplitter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridSplitter),
                new FrameworkPropertyMetadata(typeof(GridSplitter)));
        }
    }
}