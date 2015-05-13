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

namespace ModernApplicationFramework.Docking
{
    internal static class Win32Helper
    {
        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateWindowEx(int dwExStyle,
                                                      string lpszClassName,
                                                      string lpszWindowName,
                                                      int style,
                                                      int x, int y,
                                                      int width, int height,
                                                      IntPtr hwndParent,
                                                      IntPtr hMenu,
                                                      IntPtr hInst,
                                                      [MarshalAs(UnmanagedType.AsAny)] object pvParam);
        internal const int
              WsChild = 0x40000000,
              WsVisible = 0x10000000,
              WsVscroll = 0x00200000,
              WsBorder = 0x00800000,
              WsClipsiblings = 0x04000000,
              WsClipchildren = 0x02000000,
              WsTabstop = 0x00010000,
              WsGroup = 0x00020000;


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

        /// <summary>
        ///     Special window handles
        /// </summary>
        internal static readonly IntPtr HwndTopmost = new IntPtr(-1);
        internal static readonly IntPtr HwndNotopmost = new IntPtr(-2);
        internal static readonly IntPtr HwndTop = new IntPtr(0);
        internal static readonly IntPtr HwndBottom = new IntPtr(1);

        [StructLayout(LayoutKind.Sequential)]
        internal class Windowpos
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        }; 

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool IsChild(IntPtr hWndParent, IntPtr hwnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetFocus(IntPtr hWnd);

        internal const int WmWindowposchanged = 0x0047;
        internal const int WmWindowposchanging = 0x0046;
        internal const int WmNcmousemove = 0xa0;
        internal const int WmNclbuttondown = 0xA1;
        internal const int WmNclbuttonup = 0xA2;
        internal const int WmNclbuttondblclk = 0xA3;
        internal const int WmNcrbuttondown = 0xA4;
        internal const int WmNcrbuttonup = 0xA5;
        internal const int WmCapturechanged = 0x0215;
        internal const int WmExitsizemove = 0x0232;
        internal const int WmEntersizemove = 0x0231;
        internal const int WmMove = 0x0003;
        internal const int WmMoving = 0x0216;
        internal const int WmKillfocus = 0x0008;
        internal const int WmSetfocus = 0x0007;
        internal const int WmActivate = 0x0006;
        internal const int WmNchittest = 0x0084;
        internal const int WmInitmenupopup = 0x0117;
        internal const int WmKeydown = 0x0100;
        internal const int WmKeyup = 0x0101;

        internal const int WaInactive = 0x0000;

        internal const int WmSyscommand = 0x0112;
        // These are the wParam of WM_SYSCOMMAND
        internal const int ScMaximize = 0xF030;
        internal const int ScRestore = 0xF120;

        internal const int
            WmCreate = 0x0001;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        internal static extern bool DestroyWindow(IntPtr hwnd);

        internal const int HtCaption = 0x2;

        [DllImport("user32.dll")]
        internal static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        internal static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);


        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

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

        public const int HcbtSetfocus = 9;
        public const int HcbtActivate = 5;

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        public delegate int HookProc(int code, IntPtr wParam,
           IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType code,
            HookProc func,
            IntPtr hInstance,
            int threadId);
        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(IntPtr hhook);
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hhook,
            int code, IntPtr wParam, IntPtr lParam);

        [Serializable, StructLayout(LayoutKind.Sequential)]

        // ReSharper disable once InconsistentNaming
        internal struct RECT
        {
            public int Left;
            public int Top; 
            public int Right; 
            public int Bottom;
            public RECT(int left, int top, int right, int bottom)
            {
                Left = left; Top = top; Right = right; Bottom = bottom;
            }

            public int Height => Bottom - Top;

	        public int Width => Right - Left;
	        public Size Size => new Size(Width, Height);
	        public Point Location => new Point(Left, Top);
	        // Handy method for converting to a System.Drawing.Rectangle  
            public Rect ToRectangle() { return new Rect(Left, Top, Right, Bottom); } 
            public static RECT FromRectangle(Rect rectangle)
            { return new Rect(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom); }   
            public override int GetHashCode() => Left ^ ((Top << 13) | (Top >> 0x13)) ^ ((Width << 0x1a) | (Width >> 6)) ^ ((Height << 7) | (Height >> 0x19));

	        #region Operator overloads
            public static implicit operator Rect(RECT rect) { return rect.ToRectangle(); }   public static implicit operator RECT(Rect rect) { return FromRectangle(rect); }
            #endregion
        }

        internal static RECT GetClientRect(IntPtr hWnd)
        {
            RECT result;
            GetClientRect(hWnd, out result);
            return result;
        }
        internal static RECT GetWindowRect(IntPtr hWnd)
        {
            RECT result;
            GetWindowRect(hWnd, out result);
            return result;
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr GetTopWindow(IntPtr hWnd);

        internal const uint GwHwndnext = 2;
        internal const uint GwHwndprev = 3;


        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

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

        internal static int MakeLParam(int loWord, int hiWord)
        {
            return ((hiWord << 16) | (loWord & 0xffff));
        }


        internal const int WmMousemove = 0x200;
        internal const int WmLbuttondown = 0x201;
        internal const int WmLbuttonup = 0x202;
        internal const int WmLbuttondblclk = 0x203;
        internal const int WmRbuttondown = 0x204;
        internal const int WmRbuttonup = 0x205;
        internal const int WmRbuttondblclk = 0x206;
        internal const int WmMbuttondown = 0x207;
        internal const int WmMbuttonup = 0x208;
        internal const int WmMbuttondblclk = 0x209;
        internal const int WmMousewheel = 0x20A;
        internal const int WmMousehwheel = 0x20E;


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public int X;
            public int Y;
        };
        internal static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetParent(IntPtr hWnd);

        /// <summary>
        /// Changes an attribute of the specified window. The function also sets the 32-bit (long) value at the specified offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs..</param>
        /// <param name="nIndex">The zero-based offset to the value to be set. Valid values are in the range zero through the number of bytes of extra window memory, minus the size of an integer. To set any other value, specify one of the following values: GWL_EXSTYLE, GWL_HINSTANCE, GWL_ID, GWL_STYLE, GWL_USERDATA, GWL_WNDPROC </param>
        /// <param name="dwNewLong">The replacement value.</param>
        /// <returns>If the function succeeds, the return value is the previous value of the specified 32-bit integer.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError. </returns>
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public static void SetOwner(IntPtr childHandle, IntPtr ownerHandle)
        {
            SetWindowLong(
                childHandle,
                -8, // GWL_HWNDPARENT
                ownerHandle.ToInt32());
        }

        public static IntPtr GetOwner(IntPtr childHandle)
        {
            return new IntPtr(GetWindowLong(childHandle, -8));
        }


        //Monitor Patch #13440

        /// <summary>
        /// The MonitorFromRect function retrieves a handle to the display monitor that 
        /// has the largest area of intersection with a specified rectangle.
        /// </summary>
        /// <param name="lprc">Pointer to a RECT structure that specifies the rectangle of interest in 
        /// virtual-screen coordinates</param>
        /// <param name="dwFlags">Determines the function's return value if the rectangle does not intersect 
        /// any display monitor</param>
        /// <returns>
        /// If the rectangle intersects one or more display monitor rectangles, the return value 
        /// is an HMONITOR handle to the display monitor that has the largest area of intersection with the rectangle.
        /// If the rectangle does not intersect a display monitor, the return value depends on the value of dwFlags.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromRect([In] ref RECT lprc, uint dwFlags);

        /// <summary>
        /// The MonitorFromWindow function retrieves a handle to the display monitor that has the largest area of intersection with the bounding rectangle of a specified window. 
        /// </summary>
        /// <param name="hwnd">A handle to the window of interest.</param>
        /// <param name="dwFlags">Determines the function's return value if the window does not intersect any display monitor.</param>
        /// <returns>If the window intersects one or more display monitor rectangles, the return value is an HMONITOR handle to the display monitor that has the largest area of intersection with the window. 
        /// If the window does not intersect a display monitor, the return value depends on the value of dwFlags.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);


        /// <summary>
        /// The MONITORINFO structure contains information about a display monitor.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class MonitorInfo
        {
            /// <summary>
            /// The size of the structure, in bytes. 
            /// </summary>
            public int Size = Marshal.SizeOf(typeof(MonitorInfo));
            /// <summary>
            /// A RECT structure that specifies the display monitor rectangle, expressed 
            /// in virtual-screen coordinates. 
            /// Note that if the monitor is not the primary display monitor, 
            /// some of the rectangle's coordinates may be negative values. 
            /// </summary>
            public RECT Monitor;
            /// <summary>
            /// A RECT structure that specifies the work area rectangle of the display monitor, 
            /// expressed in virtual-screen coordinates. Note that if the monitor is not the primary 
            /// display monitor, some of the rectangle's coordinates may be negative values.
            /// </summary>
            public RECT Work;
            /// <summary>
            /// A set of flags that represent attributes of the display monitor. 
            /// </summary>
            public uint Flags;
        }

        /// <summary>
        /// The GetMonitorInfo function retrieves information about a display monitor. 
        /// </summary>
        /// <param name="hMonitor">Handle to the display monitor of interest.</param>
        /// <param name="lpmi">Pointer to a MONITORINFO or MONITORINFOEX structure that receives 
        /// information about the specified display monitor</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, [In, Out] MonitorInfo lpmi);

    }
}
