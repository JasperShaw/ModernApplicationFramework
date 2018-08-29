using System.Windows;
using ModernApplicationFramework.Controls.Buttons;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls.Internals
{
    public class WindowTitleBarButton : Button, INonClientArea
    {
        public static readonly DependencyProperty IsAnchorableFloatingWindowTitleBarButtonProperty = DependencyProperty.Register(
            "IsAnchorableFloatingWindowTitleBarButton", typeof(bool), typeof(WindowTitleBarButton), new PropertyMetadata(default(bool)));

        public bool IsAnchorableFloatingWindowTitleBarButton
        {
            get => (bool) GetValue(IsAnchorableFloatingWindowTitleBarButtonProperty);
            set => SetValue(IsAnchorableFloatingWindowTitleBarButtonProperty, value);
        }

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