using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Controls.Primitives
{
    internal class ResizeGrip : System.Windows.Controls.Primitives.ResizeGrip, INonClientArea
    {
        static ResizeGrip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeGrip),
                new FrameworkPropertyMetadata(typeof(ResizeGrip)));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.ChangedButton != MouseButton.Left)
                return;
            var num = FlowDirection == FlowDirection.LeftToRight ? 8 : 7;
            var hwndSource = (HwndSource) PresentationSource.FromVisual(this);
            if (hwndSource == null)
                return;
            User32.SendMessage(hwndSource.Handle, 274, (IntPtr) (61440 + num), IntPtr.Zero);
        }

        public int HitTest(Point point)
        {
            return 0;
        }
    }
}