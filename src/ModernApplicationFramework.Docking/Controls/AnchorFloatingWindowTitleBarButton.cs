using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Controls.Internals;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Docking.Controls
{
    public class AnchorFloatingWindowTitleBarButton : WindowTitleBarButton
    {
        public AnchorFloatingWindowTitleBarButton()
        {
            
        }

        static AnchorFloatingWindowTitleBarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorFloatingWindowTitleBarButton),
                new FrameworkPropertyMetadata(typeof(AnchorFloatingWindowTitleBarButton)));
        }
    }
}
