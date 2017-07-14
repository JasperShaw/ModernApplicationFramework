using System.Windows;

namespace ModernApplicationFramework.Controls.Menu
{
    public class MenuScrollViewer : ScrollViewer
    {
        static MenuScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuScrollViewer), new FrameworkPropertyMetadata(typeof(MenuScrollViewer)));
        }
    }
}
