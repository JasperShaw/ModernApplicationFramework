/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.Runtime.InteropServices;
using System.Windows;
using ModernApplicationFramework.Core.NativeMethods;

namespace ModernApplicationFramework.Docking
{
    internal static class Win32Helper
    {
        public const int HcbtActivate = 5;

        public const int HcbtSetfocus = 9;

        internal const uint GwHwndnext = 2;
        internal const uint GwHwndprev = 3;

        internal const int HtCaption = 0x2;
        // These are the wParam of WM_SYSCOMMAND
        internal const int ScMaximize = 0xF030;
        internal const int ScRestore = 0xF120;

        internal const int WaInactive = 0x0000;
        internal const int WmActivate = 0x0006;
        internal const int WmCapturechanged = 0x0215;

        internal const int
            WmCreate = 0x0001;

        internal const int WmEntersizemove = 0x0231;
        internal const int WmExitsizemove = 0x0232;
        internal const int WmInitmenupopup = 0x0117;
        internal const int WmKeydown = 0x0100;
        internal const int WmKeyup = 0x0101;
        internal const int WmKillfocus = 0x0008;
        internal const int WmLbuttondblclk = 0x203;
        internal const int WmLbuttondown = 0x201;
        internal const int WmLbuttonup = 0x202;
        internal const int WmMbuttondblclk = 0x209;
        internal const int WmMbuttondown = 0x207;
        internal const int WmMbuttonup = 0x208;
        internal const int WmMousehwheel = 0x20E;


        internal const int WmMousemove = 0x200;
        internal const int WmMousewheel = 0x20A;
        internal const int WmMove = 0x0003;
        internal const int WmMoving = 0x0216;
        internal const int WmNchittest = 0x0084;
        internal const int WmNclbuttondblclk = 0xA3;
        internal const int WmNclbuttondown = 0xA1;
        internal const int WmNclbuttonup = 0xA2;
        internal const int WmNcmousemove = 0xa0;
        internal const int WmNcrbuttondown = 0xA4;
        internal const int WmNcrbuttonup = 0xA5;
        internal const int WmRbuttondblclk = 0x206;
        internal const int WmRbuttondown = 0x204;
        internal const int WmRbuttonup = 0x205;
        internal const int WmSetfocus = 0x0007;

        internal const int WmSyscommand = 0x0112;

        internal const int WmWindowposchanged = 0x0047;
        internal const int WmWindowposchanging = 0x0046;

        internal const int
            WsChild = 0x40000000,
            WsVisible = 0x10000000,
            WsVscroll = 0x00200000,
            WsBorder = 0x00800000,
            WsClipsiblings = 0x04000000,
            WsClipchildren = 0x02000000,
            WsTabstop = 0x00010000,
            WsGroup = 0x00020000;

        internal static readonly IntPtr HwndBottom = new IntPtr(1);
        internal static readonly IntPtr HwndNotopmost = new IntPtr(-2);
        internal static readonly IntPtr HwndTop = new IntPtr(0);

        /// <summary>
        ///     Special window handles
        /// </summary>
        internal static readonly IntPtr HwndTopmost = new IntPtr(-1);

        public delegate int HookProc(int code, IntPtr wParam,
            IntPtr lParam);

        // Hook Types  
        public enum HookType
        {
            WhJournalrecord = 0,
            WhJournalplayback = 1,
            WhKeyboard = 2,
            WhGetmessage = 3,
            WhCallwndproc = 4,
            WhCbt = 5,
            WhSysmsgfilter = 6,
            WhMouse = 7,
            WhHardware = 8,
            WhDebug = 9,
            WhShell = 10,
            WhForegroundidle = 11,
            WhCallwndprocret = 12,
            WhKeyboardLl = 13,
            WhMouseLl = 14
        }

        internal enum GetWindowCmd : uint
        {
            GwHwndfirst = 0,
            GwHwndlast = 1,
            GwHwndnext = 2,
            GwHwndprev = 3,
            GwOwner = 4,
            GwChild = 5,
            GwEnabledpopup = 6
        }


        /// <summary>
        /// SetWindowPos Flags
        /// </summary>
        [Flags]
        internal enum SetWindowPosFlags : uint
        {
            /// <summary>If the calling thread and the thread that owns the window are attached to different input queues,
            /// the system posts the request to the thread that owns the window. This prevents the calling thread from
            /// blocking its execution while other threads process the request.</summary>
            /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
            SynchronousWindowPosition = 0x4000,

            /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
            /// <remarks>SWP_DEFERERASE</remarks>
            DeferErase = 0x2000,

            /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
            /// <remarks>SWP_DRAWFRAME</remarks>
            DrawFrame = 0x0020,

            /// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to
            /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE
            /// is sent only when the window's size is being changed.</summary>
            /// <remarks>SWP_FRAMECHANGED</remarks>
            FrameChanged = 0x0020,

            /// <summary>Hides the window.</summary>
            /// <remarks>SWP_HIDEWINDOW</remarks>
            HideWindow = 0x0080,

            /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the
            /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
            /// parameter).</summary>
            /// <remarks>SWP_NOACTIVATE</remarks>
            DoNotActivate = 0x0010,

            /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid
            /// contents of the client area are saved and copied back into the client area after the window is sized or
            /// repositioned.</summary>
            /// <remarks>SWP_NOCOPYBITS</remarks>
            DoNotCopyBits = 0x0100,

            /// <summary>Retains the current position (ignores X and Y parameters).</summary>
            /// <remarks>SWP_NOMOVE</remarks>
            IgnoreMove = 0x0002,

            /// <summary>Does not change the owner window's position in the Z order.</summary>
            /// <remarks>SWP_NOOWNERZORDER</remarks>
            DoNotChangeOwnerZOrder = 0x0200,

            /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to
            /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent
            /// window uncovered as a result of the window being moved. When this flag is set, the application must
            /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
            /// <remarks>SWP_NOREDRAW</remarks>
            DoNotRedraw = 0x0008,

            /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
            /// <remarks>SWP_NOREPOSITION</remarks>
            DoNotReposition = 0x0200,

            /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
            /// <remarks>SWP_NOSENDCHANGING</remarks>
            DoNotSendChangingEvent = 0x0400,

            /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
            /// <remarks>SWP_NOSIZE</remarks>
            IgnoreResize = 0x0001,

            /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
            /// <remarks>SWP_NOZORDER</remarks>
            IgnoreZOrder = 0x0004,

            /// <summary>Displays the window.</summary>
            /// <remarks>SWP_SHOWWINDOW</remarks>
            ShowWindow = 0x0040,
        }



        public static void SetOwner(IntPtr childHandle, IntPtr ownerHandle)
        {
            User32.SetWindowLong(
                childHandle,
                -8,
                ownerHandle.ToInt32());
        }



        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(IntPtr hhook);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool BringWindowToTop(IntPtr hWnd);


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        internal static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetParent(IntPtr hWnd);


        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool IsChild(IntPtr hWndParent, IntPtr hwnd);

        [DllImport("user32.dll")]
        internal static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy,
            SetWindowPosFlags uFlags);


        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public int X;
            public int Y;
        }
    }
}