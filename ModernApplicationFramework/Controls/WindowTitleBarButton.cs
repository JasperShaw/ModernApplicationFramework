using System.Windows;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls
{
    public class WindowTitleBarButton : Button, INonClientArea
    {
        static WindowTitleBarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowTitleBarButton), new FrameworkPropertyMetadata(typeof(WindowTitleBarButton)));
        }

		public int HitTest(Point point)
        {
            return 1;
        }
    }
}
