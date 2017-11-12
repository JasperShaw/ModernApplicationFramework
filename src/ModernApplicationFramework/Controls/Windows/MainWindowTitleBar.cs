using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ModernApplicationFramework.Controls.AutomationPeer;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls.Windows
{
    internal sealed class MainWindowTitleBar : Border, INonClientArea
    {
        public static readonly DependencyProperty ShowContextMenuProperty = DependencyProperty.Register(
            "ShowContextMenu", typeof(bool), typeof(MainWindowTitleBar), new PropertyMetadata(default(bool)));

        public bool ShowContextMenu
        {
            get => (bool) GetValue(ShowContextMenuProperty);
            set => SetValue(ShowContextMenuProperty, value);
        }

        public int HitTest(Point point)
        {
            return 2;
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            if (e.Handled)
                return;
            if (!ShowContextMenu)
                return;
            if (PresentationSource.FromVisual(this) is HwndSource source)
                ModernChromeWindow.ShowWindowMenu(source, this, Mouse.GetPosition(this), RenderSize);
            e.Handled = true;
        }

        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new MainWindowTitleBarAutomationPeer(this);
        }
    }
}