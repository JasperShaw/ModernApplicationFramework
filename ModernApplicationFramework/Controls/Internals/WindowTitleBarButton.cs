using System.Windows;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls.Internals
{
    internal class WindowTitleBarButton : Button, INonClientArea
    {
        public static readonly DependencyProperty IsAnchroableFloatingWindowTitleBarButtonProperty = DependencyProperty.Register(
            "IsAnchroableFloatingWindowTitleBarButton", typeof(bool), typeof(WindowTitleBarButton), new PropertyMetadata(default(bool)));

        public bool IsAnchroableFloatingWindowTitleBarButton
        {
            get => (bool) GetValue(IsAnchroableFloatingWindowTitleBarButtonProperty);
            set => SetValue(IsAnchroableFloatingWindowTitleBarButtonProperty, value);
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