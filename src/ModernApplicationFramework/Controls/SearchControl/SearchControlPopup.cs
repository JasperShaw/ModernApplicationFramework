using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using ModernApplicationFramework.Controls.Windows;

namespace ModernApplicationFramework.Controls.SearchControl
{
    public class SearchControlPopup : Popup
    {
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            if (!(Child is SystemDropShadowChrome child))
                return;

            var hwndSource = (HwndSource) PresentationSource.FromVisual(child);
            var context = Native.NativeMethods.Imm32.ImmGetContext(hwndSource.Handle);
            if (!(context !=  IntPtr.Zero))
                return;

            try
            {
                if (!Native.NativeMethods.Imm32.ImmGetOpenStatus(context))
                    return;
                Native.NativeMethods.User32.SetWindowPos(hwndSource.Handle, new IntPtr(-2), 0, 0, 0, 0, 19);
            }
            finally
            {
                Native.NativeMethods.Imm32.ImmReleaseContext(hwndSource.Handle, context);
            }
        }
    }
}
