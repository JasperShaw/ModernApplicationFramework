using System.Windows;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls.Internals
{
    internal class WindowTitleBarButton : Button, INonClientArea
    {
        static WindowTitleBarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowTitleBarButton),
                new FrameworkPropertyMetadata(typeof(WindowTitleBarButton)));
        }

        public int HitTest(Point point)
        {
            return 1;
        }
    }
}