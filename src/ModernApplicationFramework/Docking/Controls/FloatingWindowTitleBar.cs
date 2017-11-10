using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ModernApplicationFramework.Controls.AutomationPeer;
using ModernApplicationFramework.Controls.Windows;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Docking.Controls
{
    internal sealed class FloatingWindowTitleBar : Border, INonClientArea
    {
        public static readonly DependencyProperty IsWindowTitleBarProperty;

        public bool IsWindowTitleBar
        {
            get => (bool)GetValue(IsWindowTitleBarProperty);
            set => SetValue(IsWindowTitleBarProperty, Boxes.Box(value));
        }

        int INonClientArea.HitTest(Point point)
        {
            return 2;
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        static FloatingWindowTitleBar()
        {
            IsWindowTitleBarProperty = DependencyProperty.Register("IsWindowTitleBar", typeof(bool),
                typeof(FloatingWindowTitleBar), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            CaptureMouse();
            base.OnMouseRightButtonDown(e);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
                ReleaseMouseCapture();
            var source = PresentationSource.FromVisual(this) as HwndSource;
            if (source == null)
                return;
            if (IsWindowTitleBar)
                ModernChromeWindow.ShowWindowMenu(source, this, e.GetPosition(this), RenderSize);
            else
            {
                var ctxMenu = DockingManager.Instance.AnchorableContextMenu;
                if (ctxMenu != null)
                {
                    ctxMenu.PlacementTarget = null;
                    ctxMenu.Placement = PlacementMode.MousePoint;
                    ctxMenu.IsOpen = true;
                }
            }
            e.Handled = true;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new MainWindowTitleBarAutomationPeer(this);
        }
    }
}