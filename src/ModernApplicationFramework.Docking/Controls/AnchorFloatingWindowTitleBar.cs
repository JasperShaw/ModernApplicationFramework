using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ModernApplicationFramework.Controls.Windows;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Docking.Controls
{
    internal sealed class AnchorFloatingWindowTitleBar : DragUndockHeader
    {
        public static readonly DependencyProperty UseLargeTitleBarStyleProperty;

        public bool UseLargeTitleBarStyle
        {
            get => (bool)GetValue(UseLargeTitleBarStyleProperty);
            set => SetValue(UseLargeTitleBarStyleProperty, Boxes.Box(value));
        }


        static AnchorFloatingWindowTitleBar()
        {
            UseLargeTitleBarStyleProperty = DependencyProperty.Register(nameof(UseLargeTitleBarStyle), typeof(bool),
                typeof(AnchorFloatingWindowTitleBar), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        }

        protected override bool ShouldShowWindowMenu()
        {
            if (!UseLargeTitleBarStyle)
                return false;
            return base.ShouldShowWindowMenu();
        }

        //protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        //{
        //    CaptureMouse();
        //    base.OnMouseRightButtonDown(e);
        //}

        //protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        //{
        //    if (IsMouseCaptured)
        //        ReleaseMouseCapture();
        //    var source = PresentationSource.FromVisual(this) as HwndSource;
        //    if (source == null)
        //        return;
        //    if (UseLargeTitleBarStyle)
        //        ModernChromeWindow.ShowWindowMenu(source, this, e.GetPosition(this), RenderSize);
        //    else
        //    {
        //        var ctxMenu = DockingManager.Instance.AnchorableContextMenu;
        //        if (ctxMenu != null)
        //        {
        //            ctxMenu.PlacementTarget = null;
        //            ctxMenu.Placement = PlacementMode.MousePoint;
        //            ctxMenu.IsOpen = true;
        //        }
        //    }
        //    e.Handled = true;
        //}
    }
}