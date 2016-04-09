using System.Windows;

namespace ModernApplicationFramework.Controls
{
    public class ScrollBar : System.Windows.Controls.Primitives.ScrollBar
    {
        static ScrollBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollBar), new FrameworkPropertyMetadata(typeof(ScrollBar)));
        }
    }
}