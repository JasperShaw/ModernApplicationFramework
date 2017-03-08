using System.Windows;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Docking.Controls
{
    internal class DropDownTitleBarButton : DropDownButton, INonClientArea
    {
        static DropDownTitleBarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (DropDownTitleBarButton),
                new FrameworkPropertyMetadata(typeof (DropDownTitleBarButton)));
        }

        public int HitTest(Point point)
        {
            return 1;
        }
    }
}