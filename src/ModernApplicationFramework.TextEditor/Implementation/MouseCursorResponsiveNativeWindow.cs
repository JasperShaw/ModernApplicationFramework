using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using ModernApplicationFramework.TextEditor.NativeMethods;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class MouseCursorResponsiveNativeWindow : NativeWindow
    {
        private static readonly List<MouseCursorResponsiveNativeWindow> SubclassedWindows = new List<MouseCursorResponsiveNativeWindow>();

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 32)
                InputManager.Current.PrimaryMouseDevice.UpdateCursor();
            base.WndProc(ref m);
        }

        protected override void OnHandleChange()
        {
            base.OnHandleChange();
            if (!(Handle == IntPtr.Zero))
                return;
            SubclassedWindows.Remove(this);
        }

        public static void OverrideWmSetCursor(IntPtr windowHandle)
        {
            var responsiveNativeWindow = new MouseCursorResponsiveNativeWindow();
            SubclassedWindows.Add(responsiveNativeWindow);
            responsiveNativeWindow.AssignHandle(windowHandle);
        }

        public static void OverrideWmSetCursor(ElementHost elementHost)
        {
            var window = User32.GetWindow(elementHost.Handle, 5);
            if (!(window != IntPtr.Zero))
                return;
            OverrideWmSetCursor(window);
        }
    }
}