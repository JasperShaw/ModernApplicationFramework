using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Native;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Platform;
using ModernApplicationFramework.Native.Platform.Enums;
using ModernApplicationFramework.Native.Platform.Structs;
using ModernApplicationFramework.Utilities;
using DpiHelper = ModernApplicationFramework.Native.DpiHelper;
using NativeMethods = ModernApplicationFramework.Native.NativeMethods.NativeMethods;
using Point = System.Windows.Point;
using RECT = ModernApplicationFramework.Native.Platform.Structs.RECT;

namespace ModernApplicationFramework.Controls.Windows
{
    /// <inheritdoc />
    /// <summary>
    /// Window implementation that has custom chrome shadows
    /// </summary>
    /// <seealso cref="T:System.Windows.Window" />
    public class ModernChromeWindow : Window
    {
        private const int MonitorDefaulttonearest = 0x00000002;

        /// <summary>
        /// Indicates whether the window is in full screen mode
        /// </summary>
        public static readonly DependencyProperty FullScreenProperty =
            DependencyProperty.Register("FullScreen", typeof(bool), typeof(ModernChromeWindow));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius",
            typeof(int), typeof(ModernChromeWindow),
            new FrameworkPropertyMetadata(Boxes.Int32Zero, OnCornerRadiusChanged));

        /// <summary>
        /// The active glow color property
        /// </summary>
        public static readonly DependencyProperty ActiveGlowColorProperty = DependencyProperty.Register(
            "ActiveGlowColor", typeof(Color), typeof(ModernChromeWindow),
            new FrameworkPropertyMetadata(Colors.Transparent, OnGlowColorChanged));

        /// <summary>
        /// The inactive glow color property
        /// </summary>
        public static readonly DependencyProperty InactiveGlowColorProperty = DependencyProperty.Register(
            "InactiveGlowColor", typeof(Color), typeof(ModernChromeWindow),
            new FrameworkPropertyMetadata(Colors.Transparent, OnGlowColorChanged));

        public static readonly DependencyProperty NonClientFillColorProperty = DependencyProperty.Register(
            "NonClientFillColor", typeof(Color), typeof(ModernChromeWindow),
            new FrameworkPropertyMetadata(Colors.Black));

        private readonly GlowWindow[] _glowWindows = new GlowWindow[4];
        private bool _isGlowVisible;
        private bool _isNonClientStripVisible;
        private WindowState _lastState;
        private int _lastWindowPlacement;
        private Rect _logicalSizeForRestore = Rect.Empty;
        private DispatcherTimer _makeGlowVisibleTimer;
        private double _oldLeft, _oldTop, _oldWidth, _oldHeight;
        private IntPtr _ownerForActivate;
        private bool _updatingZOrder;
        private bool _useLogicalSizeForRestore;
        internal int DeferGlowChangesCount;
        private bool _wasMaximized;

        static ModernChromeWindow()
        {
            ResizeModeProperty.OverrideMetadata(typeof(ModernChromeWindow),
                new FrameworkPropertyMetadata(OnResizeModeChanged));
        }

        public int CornerRadius
        {
            get => (int) GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Color ActiveGlowColor
        {
            get => (Color) GetValue(ActiveGlowColorProperty);
            set => SetValue(ActiveGlowColorProperty, value);
        }

        public Color InactiveGlowColor
        {
            get => (Color) GetValue(InactiveGlowColorProperty);
            set => SetValue(InactiveGlowColorProperty, value);
        }

        public Color NonClientFillColor
        {
            get => (Color) GetValue(NonClientFillColorProperty);
            set => SetValue(NonClientFillColorProperty, value);
        }

        public bool FullScreen
        {
            get => (bool) GetValue(FullScreenProperty);
            set => SetValue(FullScreenProperty, value);
        }

        private static int PressedMouseButtons
        {
            get
            {
                var num = 0;
                if (NativeMethods.IsKeyPressed(1))
                    num |= 1;
                if (NativeMethods.IsKeyPressed(2))
                    num |= 2;
                if (NativeMethods.IsKeyPressed(4))
                    num |= 16;
                if (NativeMethods.IsKeyPressed(5))
                    num |= 32;
                if (NativeMethods.IsKeyPressed(6))
                    num |= 64;
                return num;
            }
        }

        private bool IsGlowVisible
        {
            get => _isGlowVisible;
            set
            {
                if (_isGlowVisible == value)
                    return;
                _isGlowVisible = value;
                for (var direction = 0; direction < _glowWindows.Length; ++direction)
                    GetOrCreateGlowWindow(direction).IsVisible = value;
            }
        }

        private IEnumerable<GlowWindow> LoadedGlowWindows
        {
            get { return _glowWindows.Where(w => w != null); }
        }

        protected virtual bool ShouldShowGlow
        {
            get
            {
                var handle = new WindowInteropHelper(this).Handle;
                if (User32.IsWindowVisible(handle) && !User32.IsIconic(handle) &&
                    !User32.IsZoomed(handle))
                    return (uint) ResizeMode > 0U;
                return false;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == FullScreenProperty)
                ChangeFullScreenApperance(e.NewValue);
        }

        private void ChangeFullScreenApperance(object newValue)
        {
            if ((bool) newValue)
                ChangeToFullScreen();
            else
                RestoreToOldScreen();
        }

        private void ChangeToFullScreen()
        {
            if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
            {
                _lastState = WindowState;
                WindowState = WindowState.Normal;
            }
            _oldLeft = Left;
            _oldTop = Top;
            _oldWidth = Width;
            _oldHeight = Height;

            var monitorinfo = NativeMethods.MonitorInfoFromWindow(this);
            var monitor = monitorinfo.RcMonitor;
            Left = monitor.Left;
            Top = monitor.Top;
            Width = monitor.Width;
            Height = monitor.Height;
        }

        private void RestoreToOldScreen()
        {
            ClearValue(WindowStyleProperty);
            ClearValue(ResizeModeProperty);
            ClearValue(MaxWidthProperty);
            ClearValue(MaxHeightProperty);
            WindowState = _lastState;

            Left = _oldLeft;
            Top = _oldTop;
            Width = _oldWidth;
            Height = _oldHeight;
        }

        protected override void OnActivated(EventArgs e)
        {
            UpdateGlowActiveState();
            base.OnActivated(e);
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        protected override void OnDeactivated(EventArgs e)
        {
            UpdateGlowActiveState();
            base.OnDeactivated(e);
        }

        private static void OnResizeModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((ModernChromeWindow) obj).DestroyGlowWindows();
            ((ModernChromeWindow) obj).CreateGlowWindowHandlesNoResize();
            ((ModernChromeWindow) obj).UpdateGlowVisibility(false);
        }

        private static void OnCornerRadiusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((ModernChromeWindow) obj).UpdateClipRegion();
        }

        private static void OnGlowColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((ModernChromeWindow) obj).UpdateGlowColors();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            var hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            hwndSource?.AddHook(HwndSourceHook);
            CreateGlowWindowHandles();
            KeyboardInputService.Instance?.Register(this);
            base.OnSourceInitialized(e);
        }

        private void CreateGlowWindowHandles()
        {
            for (var direction = 0; direction < _glowWindows.Length; ++direction)
                GetOrCreateGlowWindow(direction).EnsureHandle();
        }

        private void CreateGlowWindowHandlesNoResize()
        {
            for (var direction = 0; direction < _glowWindows.Length; ++direction)
                GetOrCreateGlowWindow(direction, false).EnsureHandle();
        }

        protected virtual IntPtr HwndSourceHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 174:
                case 175:
                    handled = true;
                    break;
                case 274:
                    WmSysCommand(hWnd, wParam);
                    break;
                case 128:
                case 12:
                    return CallDefWindowProcWithoutVisibleStyle(hWnd, msg, wParam, lParam, ref handled);
                case 131:
                    return WmNcCalcSize(hWnd, wParam, lParam, ref handled);
                case 132:
                    return WmNcHitTest(lParam, ref handled);
                case 133:
                    return WmNcPaint(hWnd, wParam, ref handled);
                case 134:
                    return WmNcActivate(hWnd, wParam, ref handled);
                case 164:
                case 165:
                case 166:
                    RaiseNonClientMouseMessageAsClient(hWnd, msg, lParam);
                    handled = true;
                    break;
                case 70:
                    WmWindowPosChanging(lParam);
                    break;
                case 71:
                    WmWindowPosChanged(hWnd, lParam);
                    break;
                case 6:
                    WmActivate(wParam, lParam);
                    break;
                case 36: // GetMinMaxInfo
                    WnGetMinMaxInfo(hWnd, lParam);
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        private void WnGetMinMaxInfo(IntPtr hwnd, IntPtr lparam)
        {
            var mmi = (Minmaxinfo) Marshal.PtrToStructure(lparam, typeof(Minmaxinfo));
            var monitor = User32.MonitorFromWindow(hwnd, MonitorDefaulttonearest);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MonitorInfo();
                User32.GetMonitorInfo(monitor, monitorInfo);
                var rcWorkArea = monitorInfo.rcWork;
                var rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);

                mmi.ptMinTrackSize.X = (int) MinWidth; //minimum drag X size for the window
                mmi.ptMinTrackSize.Y = (int) MinHeight; //minimum drag Y size for the window 

                mmi = AdjustWorkingAreaForAutoHide(monitor, mmi);
                //need to adjust sizing if taskbar is set to autohide   
            }
            Marshal.StructureToPtr(mmi, lparam, true);
        }

        private static Minmaxinfo AdjustWorkingAreaForAutoHide(IntPtr monitor, Minmaxinfo mmi)
        {
            var hwnd = User32.FindWindow("Shell_TrayWnd", null);
            var monitorWithTaskBar = User32.MonitorFromWindow(hwnd, MonitorDefaulttonearest);
            if (!monitor.Equals(monitorWithTaskBar))
                return mmi;
            var abd = new Appbardata();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = hwnd;
            Shell32.SHAppBarMessage((int) AbMsg.AbmGettaskbarpos, ref abd);
            var uEdge = GetEdge(abd.rc);
            var autoHide = Convert.ToBoolean(Shell32.SHAppBarMessage((int) AbMsg.AbmGetstate, ref abd));

            if (!autoHide)
                return mmi;

            switch (uEdge)
            {
                case (int) AbEdge.AbeLeft:
                    mmi.ptMaxPosition.X += 2;
                    mmi.ptMaxTrackSize.X -= 2;
                    mmi.ptMaxSize.X -= 2;
                    break;
                case (int) AbEdge.AbeRight:
                    mmi.ptMaxSize.X -= 2;
                    mmi.ptMaxTrackSize.X -= 2;
                    break;
                case (int) AbEdge.AbeTop:
                    mmi.ptMaxPosition.Y += 2;
                    mmi.ptMaxTrackSize.Y -= 2;
                    mmi.ptMaxSize.Y -= 2;
                    break;
                case (int) AbEdge.AbeBottom:
                    mmi.ptMaxSize.Y -= 2;
                    mmi.ptMaxTrackSize.Y -= 2;
                    break;
                default:
                    return mmi;
            }
            return mmi;
        }

        private static int GetEdge(RECT rc)
        {
            int uEdge;
            if (rc.Top == rc.Left && rc.Bottom > rc.Right)
                uEdge = (int) AbEdge.AbeLeft;
            else if (rc.Top == rc.Left && rc.Bottom < rc.Right)
                uEdge = (int) AbEdge.AbeTop;
            else if (rc.Top > rc.Left)
                uEdge = (int) AbEdge.AbeBottom;
            else
                uEdge = (int) AbEdge.AbeRight;
            return uEdge;
        }

        private static void RaiseNonClientMouseMessageAsClient(IntPtr hWnd, int msg, IntPtr lParam)
        {
            var point = new Native.Platform.Structs.Point
            {
                X = NativeMethods.GetXlParam(lParam.ToInt32()),
                Y = NativeMethods.GetYlParam(lParam.ToInt32())
            };
            User32.ScreenToClient(hWnd, ref point);
            User32.SendMessage(hWnd, msg + 513 - 161, new IntPtr(PressedMouseButtons),
                NativeMethods.MakeParam(point.X, point.Y));
        }

        private IntPtr CallDefWindowProcWithoutVisibleStyle(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam,
            ref bool handled)
        {
            var flag = VisualUtilities.ModifyStyle(hWnd, 268435456, 0);
            var num = User32.DefWindowProc(hWnd, msg, wParam, lParam);
            if (flag)
                VisualUtilities.ModifyStyle(hWnd, 0, 268435456);
            handled = true;
            return num;
        }

        private void WmActivate(IntPtr wParam, IntPtr lParam)
        {
            if (!(_ownerForActivate != IntPtr.Zero))
                return;
            User32.SendMessage(_ownerForActivate, NativeMethods.NotifyOwnerActivate, wParam, lParam);
        }

        private IntPtr WmNcActivate(IntPtr hWnd, IntPtr wParam, ref bool handled)
        {
            handled = true;
            return User32.DefWindowProc(hWnd, 134, wParam, new IntPtr(-1));
        }

        private IntPtr WmNcPaint(IntPtr hWnd, IntPtr wParam, ref bool handled)
        {
            if (_isNonClientStripVisible)
            {
                var hrgnClip = wParam == new IntPtr(1) ? IntPtr.Zero : wParam;
                var dcEx = User32.GetDCEx(hWnd, hrgnClip, 155);
                if (dcEx != IntPtr.Zero)
                    try
                    {
                        var nonClientFillColor = NonClientFillColor;
                        var solidBrush =
                            Gdi32.CreateSolidBrush((nonClientFillColor.B << 16) | (nonClientFillColor.G << 8) |
                                                           nonClientFillColor.R);
                        try
                        {
                            var relativeToWindowRect = GetClientRectRelativeToWindowRect(hWnd);
                            relativeToWindowRect.Top = relativeToWindowRect.Bottom;
                            relativeToWindowRect.Bottom = relativeToWindowRect.Top + 1;
                            User32.FillRect(dcEx, ref relativeToWindowRect, solidBrush);
                        }
                        finally
                        {
                            Gdi32.DeleteObject(solidBrush);
                        }
                    }
                    finally
                    {
                        User32.ReleaseDC(hWnd, dcEx);
                    }
            }
            handled = true;
            return IntPtr.Zero;
        }

        private static RECT GetClientRectRelativeToWindowRect(IntPtr hWnd)
        {
            RECT lpRect1;
            User32.GetWindowRect(hWnd, out lpRect1);
            RECT lpRect2;
            User32.GetClientRect(hWnd, out lpRect2);
            var point = new Native.Platform.Structs.Point {X = 0, Y = 0};
            User32.ClientToScreen(hWnd, ref point);
            lpRect2.Offset(point.X - lpRect1.Left, point.Y - lpRect1.Top);
            return lpRect2;
        }

        private IntPtr WmNcCalcSize(IntPtr hWnd, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            _isNonClientStripVisible = false;
            if (NativeMethods.GetWindowPlacement(hWnd).showCmd == 3)
            {
                var structure1 = (RECT) Marshal.PtrToStructure(lParam, typeof(RECT));
                User32.DefWindowProc(hWnd, 131, wParam, lParam);
                var structure2 = (RECT) Marshal.PtrToStructure(lParam, typeof(RECT));
                var monitorinfo = NativeMethods.MonitorInfoFromWindow(hWnd);
                if (monitorinfo.RcMonitor.Height == monitorinfo.RcWork.Height &&
                    monitorinfo.RcMonitor.Width == monitorinfo.RcWork.Width)
                {
                    _isNonClientStripVisible = true;
                    --structure2.Bottom;
                }
                structure2.Top = structure1.Top + (int) GetWindowInfo(hWnd).CyWindowBorders;
                Marshal.StructureToPtr((object) structure2, lParam, true);
            }
            handled = true;
            return IntPtr.Zero;
        }

        private IntPtr WmNcHitTest(IntPtr lParam, ref bool handled)
        {
            if (!this.IsConnectedToPresentationSource())
                return new IntPtr(0);
            var point1 = new Point(NativeMethods.GetXlParam(lParam.ToInt32()),
                NativeMethods.GetYlParam(lParam.ToInt32()));
            var point2 = PointFromScreen(point1);
            DependencyObject visualHit = null;
            VisualUtilities.HitTestVisibleElements(this, target =>
            {
                visualHit = target.VisualHit;
                return HitTestResultBehavior.Stop;
            }, new PointHitTestParameters(point2));
            var num = 0;
            for (; visualHit != null; visualHit = visualHit.GetVisualOrLogicalParent())
            {
                var nonClientArea = visualHit as INonClientArea;
                if (nonClientArea != null)
                {
                    num = nonClientArea.HitTest(point1);
                    if (num != 0)
                        break;
                }
            }
            if (num == 0)
                num = 1;
            handled = true;
            return new IntPtr(num);
        }

        private void WmSysCommand(IntPtr hWnd, IntPtr wParam)
        {
            var scWparam = NativeMethods.GetScWparam(wParam);
            //if (scWparam == 61456)
            //    User32.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero,
            //        RedrawWindowFlags.Invalidate | RedrawWindowFlags.NoChildren | RedrawWindowFlags.UpdateNow |
            //        RedrawWindowFlags.Frame);
            //if ((scWparam == 61488 || scWparam == 61472 || scWparam == 61456 || scWparam == 61440) &&
            //    WindowState == WindowState.Normal && !IsAeroSnappedToMonitor(hWnd))
            //    _logicalSizeForRestore = new Rect(Left, Top, Width, Height);
            //if (scWparam == 61456 && WindowState == WindowState.Maximized && _logicalSizeForRestore == Rect.Empty)
            //    _logicalSizeForRestore = new Rect(Left, Top, Width, Height);
            //if (scWparam != 61728 || WindowState == WindowState.Minimized || _logicalSizeForRestore.Width <= 0.0 ||
            //    _logicalSizeForRestore.Height <= 0.0)
            //    return;
            //Left = _logicalSizeForRestore.Left;
            //Top = _logicalSizeForRestore.Top;
            //Width = _logicalSizeForRestore.Width;
            //Height = _logicalSizeForRestore.Height;
            //_useLogicalSizeForRestore = true;


            _lastState = WindowState;
            if (scWparam == 61456)
            {
                User32.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero,
                    RedrawWindowFlags.Invalidate | RedrawWindowFlags.NoChildren | RedrawWindowFlags.UpdateNow |
                    RedrawWindowFlags.Frame);
            }
            if (_wasMaximized)
            {
                WindowStyle = WindowStyle.None;
                _wasMaximized = false;
                return;
            }
            if (scWparam == 61472 || scWparam == 61728)
                WindowStyle = WindowStyle.SingleBorderWindow;
            else
                WindowStyle = WindowStyle.None;
            if (scWparam == 61472 && WindowState == WindowState.Maximized)
                _wasMaximized = true;

            if ((scWparam == 61488 || scWparam == 61472 || scWparam == 61456 || scWparam == 61440) &&
                WindowState == WindowState.Normal && !IsAeroSnappedToMonitor(hWnd))
                _logicalSizeForRestore = new Rect(Left, Top, Width, Height);
            if (scWparam == 61456 && WindowState == WindowState.Maximized && _logicalSizeForRestore == Rect.Empty)
                _logicalSizeForRestore = new Rect(Left, Top, Width, Height);
            if (scWparam != 61728 || WindowState == WindowState.Minimized ||
                (_logicalSizeForRestore.Width <= 0.0 || _logicalSizeForRestore.Height <= 0.0))
                return;
            Left = _logicalSizeForRestore.Left;
            Top = _logicalSizeForRestore.Top;
            Width = _logicalSizeForRestore.Width;
            Height = _logicalSizeForRestore.Height;
            _useLogicalSizeForRestore = true;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowStyle = WindowStyle.None;
            base.OnStateChanged(e);
        }

        private bool IsAeroSnappedToMonitor(IntPtr hWnd)
        {
            var monitorinfo = NativeMethods.MonitorInfoFromWindow(hWnd);
            var deviceUnits = new Rect(Left, Top, Width, Height).LogicalToDeviceUnits();
            return monitorinfo.RcWork.Height == deviceUnits.Height && monitorinfo.RcWork.Top == deviceUnits.Top;
        }

        private void WmWindowPosChanging(IntPtr lParam)
        {
            var structure = (Windowpos) Marshal.PtrToStructure(lParam, typeof(Windowpos));
            if ((structure.flags & 2) != 0 || (structure.flags & 1) != 0 || structure.cx <= 0 || structure.cy <= 0)
                return;
            var floatRect = new Rect(structure.x, structure.y, structure.cx, structure.cy).DeviceToLogicalUnits();
            if (_useLogicalSizeForRestore)
            {
                floatRect = _logicalSizeForRestore;
                _logicalSizeForRestore = Rect.Empty;
                _useLogicalSizeForRestore = false;
            }
            var deviceUnits = Screen.GetOnScreenPosition(floatRect).LogicalToDeviceUnits();
            structure.x = (int) deviceUnits.X;
            structure.y = (int) deviceUnits.Y;
            Marshal.StructureToPtr((object) structure, lParam, true);
        }

        private void WmWindowPosChanged(IntPtr hWnd, IntPtr lParam)
        {
            try
            {
                var structure = (Windowpos) Marshal.PtrToStructure(lParam, typeof(Windowpos));
                var windowPlacement = NativeMethods.GetWindowPlacement(hWnd);
                var currentBounds = new RECT(structure.x, structure.y, structure.x + structure.cx,
                    structure.y + structure.cy);
                if (((int) structure.flags & 1) != 1)
                    UpdateClipRegion(hWnd, windowPlacement, ClipRegionChangeType.FromSize, currentBounds);
                else if (((int) structure.flags & 2) != 2)
                    UpdateClipRegion(hWnd, windowPlacement, ClipRegionChangeType.FromPosition, currentBounds);
                OnWindowPosChanged(hWnd, windowPlacement.showCmd, windowPlacement.rcNormalPosition.ToInt32Rect());
                UpdateGlowWindowPositions(((int) structure.flags & 64) == 0);
                UpdateZOrderOfThisAndOwner();
            }
            catch (Win32Exception)
            {
            }
        }

        private void UpdateZOrderOfThisAndOwner()
        {
            if (_updatingZOrder)
                return;
            try
            {
                _updatingZOrder = true;
                var windowInteropHelper = new WindowInteropHelper(this);
                var handle = windowInteropHelper.Handle;
                foreach (var loadedGlowWindow in LoadedGlowWindows)
                {
                    if (User32.GetWindow(loadedGlowWindow.Handle, 3) != handle)
                        User32.SetWindowPos(loadedGlowWindow.Handle, handle, 0, 0, 0, 0, 19);
                    handle = loadedGlowWindow.Handle;
                }
                var owner = windowInteropHelper.Owner;
                if (!(owner != IntPtr.Zero))
                    return;
                UpdateZOrderOfOwner(owner);
            }
            finally
            {
                _updatingZOrder = false;
            }
        }

        private void UpdateZOrderOfOwner(IntPtr hwndOwner)
        {
            var lastOwnedWindow = IntPtr.Zero;
            User32.EnumThreadWindows(Kernel32.GetCurrentThreadId(), (hwnd, lParam) =>
            {
                if (User32.GetWindow(hwnd, 4) == hwndOwner)
                    lastOwnedWindow = hwnd;
                return true;
            }, IntPtr.Zero);
            if (!(lastOwnedWindow != IntPtr.Zero) || !(User32.GetWindow(hwndOwner, 3) != lastOwnedWindow))
                return;
            User32.SetWindowPos(hwndOwner, lastOwnedWindow, 0, 0, 0, 0, 19);
        }

        protected virtual void OnWindowPosChanged(IntPtr hWnd, int showCmd, Int32Rect rcNormalPosition)
        {
        }

        protected void UpdateClipRegion(ClipRegionChangeType regionChangeType = ClipRegionChangeType.FromPropertyChange)
        {
            var hwndSource = (HwndSource) PresentationSource.FromVisual(this);
            if (hwndSource == null)
                return;
            RECT lpRect;
            User32.GetWindowRect(hwndSource.Handle, out lpRect);
            var windowPlacement = NativeMethods.GetWindowPlacement(hwndSource.Handle);
            UpdateClipRegion(hwndSource.Handle, windowPlacement, regionChangeType, lpRect);
        }

        private void UpdateClipRegion(IntPtr hWnd, Windowplacement placement, ClipRegionChangeType changeType,
            RECT currentBounds)
        {
            UpdateClipRegionCore(hWnd, placement.showCmd, changeType, currentBounds.ToInt32Rect());
            _lastWindowPlacement = placement.showCmd;
        }

        protected virtual bool UpdateClipRegionCore(IntPtr hWnd, int showCmd, ClipRegionChangeType changeType,
            Int32Rect currentBounds)
        {
            if (showCmd == 3)
            {
                UpdateMaximizedClipRegion(hWnd);
                return true;
            }
            if (changeType != ClipRegionChangeType.FromSize && changeType != ClipRegionChangeType.FromPropertyChange &&
                _lastWindowPlacement == showCmd)
                return false;
            if (CornerRadius < 0)
                ClearClipRegion(hWnd);
            else
                SetRoundRect(hWnd, currentBounds.Width, currentBounds.Height);
            return true;
        }

        private Windowinfo GetWindowInfo(IntPtr hWnd)
        {
            var pwi = new Windowinfo();
            pwi.CbSize = Marshal.SizeOf((object) pwi);
            User32.GetWindowInfo(hWnd, ref pwi);
            return pwi;
        }

        private void UpdateMaximizedClipRegion(IntPtr hWnd)
        {
            var relativeToWindowRect = GetClientRectRelativeToWindowRect(hWnd);
            if (_isNonClientStripVisible)
                ++relativeToWindowRect.Bottom;
            var rectRgnIndirect = Gdi32.CreateRectRgnIndirect(ref relativeToWindowRect);
            User32.SetWindowRgn(hWnd, rectRgnIndirect, User32.IsWindowVisible(hWnd));
        }

        private static void ClearClipRegion(IntPtr hWnd)
        {
            User32.SetWindowRgn(hWnd, IntPtr.Zero, User32.IsWindowVisible(hWnd));
        }

        protected void SetRoundRect(IntPtr hWnd, int width, int height)
        {
            var roundRectRegion = ComputeRoundRectRegion(0, 0, width, height, CornerRadius);
            User32.SetWindowRgn(hWnd, roundRectRegion, User32.IsWindowVisible(hWnd));
        }

        private IntPtr ComputeRoundRectRegion(int left, int top, int width, int height, int cornerRadius)
        {
            var nWidthEllipse = (int) (2 * cornerRadius * DpiHelper.LogicalToDeviceUnitsScalingFactorX);
            var nHeightEllipse = (int) (2 * cornerRadius * DpiHelper.LogicalToDeviceUnitsScalingFactorY);
            return Gdi32.CreateRoundRectRgn(left, top, left + width + 1, top + height + 1, nWidthEllipse,
                nHeightEllipse);
        }

        protected IntPtr ComputeCornerRadiusRectRegion(Int32Rect rect, CornerRadius cornerRadius)
        {
            if (cornerRadius.TopLeft == cornerRadius.TopRight && cornerRadius.TopLeft == cornerRadius.BottomLeft &&
                cornerRadius.BottomLeft == cornerRadius.BottomRight)
                return ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (int) cornerRadius.TopLeft);
            var num1 = IntPtr.Zero;
            var num2 = IntPtr.Zero;
            var num3 = IntPtr.Zero;
            var num4 = IntPtr.Zero;
            var num5 = IntPtr.Zero;
            var num6 = IntPtr.Zero;
            var num7 = IntPtr.Zero;
            var num8 = IntPtr.Zero;
            var num9 = IntPtr.Zero;
            IntPtr num10;
            try
            {
                num1 = ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (int) cornerRadius.TopLeft);
                num2 = ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (int) cornerRadius.TopRight);
                num3 = ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (int) cornerRadius.BottomLeft);
                num4 = ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (int) cornerRadius.BottomRight);
                var point = new Native.Platform.Structs.Point()
                {
                    X = rect.X + rect.Width / 2,
                    Y = rect.Y + rect.Height / 2
                };
                num5 = Gdi32.CreateRectRgn(rect.X, rect.Y, point.X + 1, point.Y + 1);
                num6 = Gdi32.CreateRectRgn(point.X - 1, rect.Y, rect.X + rect.Width, point.Y + 1);
                num7 = Gdi32.CreateRectRgn(rect.X, point.Y - 1, point.X + 1, rect.Y + rect.Height);
                num8 = Gdi32.CreateRectRgn(point.X - 1, point.Y - 1, rect.X + rect.Width, rect.Y + rect.Height);
                num9 = Gdi32.CreateRectRgn(0, 0, 1, 1);
                num10 = Gdi32.CreateRectRgn(0, 0, 1, 1);
                NativeMethods.CombineRgn(num10, num1, num5, CombineMode.RgnAnd);
                NativeMethods.CombineRgn(num9, num2, num6, CombineMode.RgnAnd);
                NativeMethods.CombineRgn(num10, num10, num9, CombineMode.RgnOr);
                NativeMethods.CombineRgn(num9, num3, num7, CombineMode.RgnAnd);
                NativeMethods.CombineRgn(num10, num10, num9, CombineMode.RgnOr);
                NativeMethods.CombineRgn(num9, num4, num8, CombineMode.RgnAnd);
                NativeMethods.CombineRgn(num10, num10, num9, CombineMode.RgnOr);
            }
            finally
            {
                if (num1 != IntPtr.Zero)
                    Gdi32.DeleteObject(num1);
                if (num2 != IntPtr.Zero)
                    Gdi32.DeleteObject(num2);
                if (num3 != IntPtr.Zero)
                    Gdi32.DeleteObject(num3);
                if (num4 != IntPtr.Zero)
                    Gdi32.DeleteObject(num4);
                if (num5 != IntPtr.Zero)
                    Gdi32.DeleteObject(num5);
                if (num6 != IntPtr.Zero)
                    Gdi32.DeleteObject(num6);
                if (num7 != IntPtr.Zero)
                    Gdi32.DeleteObject(num7);
                if (num8 != IntPtr.Zero)
                    Gdi32.DeleteObject(num8);
                if (num9 != IntPtr.Zero)
                    Gdi32.DeleteObject(num9);
            }
            return num10;
        }

        public static void ShowWindowMenu(HwndSource source, Visual element, Point elementPoint, Size elementSize)
        {
            if (elementPoint.X < 0.0 || elementPoint.X > elementSize.Width || elementPoint.Y < 0.0 ||
                elementPoint.Y > elementSize.Height)
                return;
            var screen = element.PointToScreen(elementPoint);
            ShowWindowMenu(source, screen, true);
        }

        protected static void ShowWindowMenu(HwndSource source, Point screenPoint, bool canMinimize)
        {
            var systemMetrics = User32.GetSystemMetrics(40);
            var systemMenu = User32.GetSystemMenu(source.Handle, false);
            var windowPlacement = NativeMethods.GetWindowPlacement(source.Handle);
            var flag = VisualUtilities.ModifyStyle(source.Handle, 268435456, 0);
            var num1 = canMinimize ? 0U : 1U;
            if (windowPlacement.showCmd == 1)
            {
                User32.EnableMenuItem(systemMenu, 61728U, 1U);
                User32.EnableMenuItem(systemMenu, 61456U, 0U);
                User32.EnableMenuItem(systemMenu, 61440U, 0U);
                User32.EnableMenuItem(systemMenu, 61488U, 0U);
                User32.EnableMenuItem(systemMenu, 61472U, 0U | num1);
                User32.EnableMenuItem(systemMenu, 61536U, 0U);
            }
            else if (windowPlacement.showCmd == 3)
            {
                User32.EnableMenuItem(systemMenu, 61728U, 0U);
                User32.EnableMenuItem(systemMenu, 61456U, 1U);
                User32.EnableMenuItem(systemMenu, 61440U, 1U);
                User32.EnableMenuItem(systemMenu, 61488U, 1U);
                User32.EnableMenuItem(systemMenu, 61472U, 0U | num1);
                User32.EnableMenuItem(systemMenu, 61536U, 0U);
            }
            if (flag)
                VisualUtilities.ModifyStyle(source.Handle, 0, 268435456);
            var fuFlags = (uint) (systemMetrics | 256 | 128 | 2);
            var num2 = User32.TrackPopupMenuEx(systemMenu, fuFlags, (int) screenPoint.X, (int) screenPoint.Y,
                source.Handle, IntPtr.Zero);
            if (num2 == 0)
                return;
            User32.PostMessage(source.Handle, 274, new IntPtr(num2), IntPtr.Zero);
        }

        protected override void OnClosed(EventArgs e)
        {
            StopTimer();
            DestroyGlowWindows();
            KeyboardInputService.Instance?.Unregister(this);
            base.OnClosed(e);
        }

        private GlowWindow GetOrCreateGlowWindow(int direction, bool isSubclass = true)
        {
            return _glowWindows[direction] ?? (_glowWindows[direction] = new GlowWindow(this, (Dock) direction)
            {
                ActiveGlowColor = ActiveGlowColor,
                InactiveGlowColor = InactiveGlowColor,
                IsActive = IsActive,
                IsWindowSubclassed = isSubclass
            });
        }

        private void DestroyGlowWindows()
        {
            for (var index = 0; index < _glowWindows.Length; ++index)
                using (_glowWindows[index])
                {
                    _glowWindows[index] = null;
                }
        }

        private void UpdateGlowWindowPositions(bool delayIfNecessary)
        {
            using (DeferGlowChanges())
            {
                UpdateGlowVisibility(delayIfNecessary);
                foreach (var loadedGlowWindow in LoadedGlowWindows)
                    loadedGlowWindow.UpdateWindowPos();
            }
        }

        private void UpdateGlowActiveState()
        {
            using (DeferGlowChanges())
            {
                foreach (var loadedGlowWindow in LoadedGlowWindows)
                    loadedGlowWindow.IsActive = IsActive;
            }
        }

        public void ChangeOwnerForActivate(IntPtr newOwner)
        {
            _ownerForActivate = newOwner;
        }

        public void ChangeOwner(IntPtr newOwner)
        {
            new WindowInteropHelper(this).Owner = newOwner;
            foreach (var loadedGlowWindow in LoadedGlowWindows)
                loadedGlowWindow.ChangeOwner(newOwner);
            UpdateZOrderOfThisAndOwner();
        }

        private void UpdateGlowVisibility(bool delayIfNecessary)
        {
            var shouldShowGlow = ShouldShowGlow;
            if (shouldShowGlow == IsGlowVisible)
                return;
            if (SystemParameters.MinimizeAnimation & shouldShowGlow & delayIfNecessary)
            {
                if (_makeGlowVisibleTimer != null)
                {
                    _makeGlowVisibleTimer.Stop();
                }
                else
                {
                    _makeGlowVisibleTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(200.0)};
                    _makeGlowVisibleTimer.Tick += OnDelayedVisibilityTimerTick;
                }
                _makeGlowVisibleTimer.Start();
            }
            else
            {
                StopTimer();
                IsGlowVisible = shouldShowGlow;
            }
        }

        private void StopTimer()
        {
            if (_makeGlowVisibleTimer == null)
                return;
            _makeGlowVisibleTimer.Stop();
            _makeGlowVisibleTimer.Tick -= OnDelayedVisibilityTimerTick;
            _makeGlowVisibleTimer = null;
        }

        private void OnDelayedVisibilityTimerTick(object sender, EventArgs e)
        {
            StopTimer();
            UpdateGlowWindowPositions(false);
        }

        private void UpdateGlowColors()
        {
            using (DeferGlowChanges())
            {
                foreach (var loadedGlowWindow in LoadedGlowWindows)
                {
                    loadedGlowWindow.ActiveGlowColor = ActiveGlowColor;
                    loadedGlowWindow.InactiveGlowColor = InactiveGlowColor;
                }
            }
        }

        private IDisposable DeferGlowChanges()
        {
            return new ChangeScope(this);
        }

        internal void EndDeferGlowChanges()
        {
            foreach (var loadedGlowWindow in LoadedGlowWindows)
                loadedGlowWindow.CommitChanges();
        }

        public virtual void ChangeTheme(Theme oldValue, Theme newValue)
        {
            UpdateGlowColors();
            IsGlowVisible = false;
            IsGlowVisible = true;
            UpdateClipRegion();
        }

        protected enum ClipRegionChangeType
        {
            FromSize,
            FromPosition,
            FromPropertyChange,
            FromUndockSingleTab
        }
    }

    internal sealed class GlowWindow : HwndWrapper
    {
        private static ushort _sharedWindowClassAtom;
        private static NativeMethods.WndProc _sharedWndProc;
        private static long _createdGlowWindows;
        private static long _disposedGlowWindows;
        private readonly GlowBitmap[] _activeGlowBitmaps = new GlowBitmap[16];
        private readonly GlowBitmap[] _inactiveGlowBitmaps = new GlowBitmap[16];
        private readonly Dock _orientation;
        private readonly ModernChromeWindow _targetWindow;
        private Color _activeGlowColor = Colors.Transparent;
        private int _height;
        private Color _inactiveGlowColor = Colors.Transparent;
        private FieldInvalidationTypes _invalidatedValues;
        private bool _isActive;
        private bool _isVisible;
        private int _left;
        private bool _pendingDelayRender;
        private int _top;
        private int _width;

        public GlowWindow(ModernChromeWindow owner, Dock orientation)
        {
            Validate.IsNotNull(owner, "owner");
            _targetWindow = owner;
            _orientation = orientation;
            ++_createdGlowWindows;
            IsWindowSubclassed = true;
        }

        private bool IsDeferringChanges => _targetWindow.DeferGlowChangesCount > 0;

        private static ushort SharedWindowClassAtom
        {
            get
            {
                if (_sharedWindowClassAtom != 0)
                    return _sharedWindowClassAtom;
                var lpWndClass = new WndClass
                {
                    cbClsExtra = 0,
                    cbWndExtra = 0,
                    hbrBackground = IntPtr.Zero,
                    hCursor = IntPtr.Zero,
                    hIcon = IntPtr.Zero,
                    lpfnWndProc = _sharedWndProc = User32.DefWindowProc,
                    lpszClassName = "ModernApplicationGlowWindow",
                    lpszMenuName = null,
                    style = 0U
                };
                _sharedWindowClassAtom = User32.RegisterClass(ref lpWndClass);
                return _sharedWindowClassAtom;
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => UpdateProperty(ref _isVisible, value,
                FieldInvalidationTypes.Render | FieldInvalidationTypes.Visibility);
        }

        public int Left
        {
            get => _left;
            set => UpdateProperty(ref _left, value, FieldInvalidationTypes.Location);
        }

        public int Top
        {
            get => _top;
            set => UpdateProperty(ref _top, value, FieldInvalidationTypes.Location);
        }

        public int Width
        {
            get => _width;
            set => UpdateProperty(ref _width, value, FieldInvalidationTypes.Size | FieldInvalidationTypes.Render);
        }

        public int Height
        {
            get => _height;
            set => UpdateProperty(ref _height, value, FieldInvalidationTypes.Size | FieldInvalidationTypes.Render);
        }

        public bool IsActive
        {
            get => _isActive;
            set => UpdateProperty(ref _isActive, value, FieldInvalidationTypes.Render);
        }

        public Color ActiveGlowColor
        {
            get => _activeGlowColor;
            set => UpdateProperty(ref _activeGlowColor, value,
                FieldInvalidationTypes.ActiveColor | FieldInvalidationTypes.Render);
        }

        public Color InactiveGlowColor
        {
            get => _inactiveGlowColor;
            set => UpdateProperty(ref _inactiveGlowColor, value,
                FieldInvalidationTypes.InactiveColor | FieldInvalidationTypes.Render);
        }

        private IntPtr TargetWindowHandle => new WindowInteropHelper(_targetWindow).Handle;

        private bool IsPositionValid => !InvalidatedValuesHasFlag(
            FieldInvalidationTypes.Location | FieldInvalidationTypes.Size | FieldInvalidationTypes.Visibility);

        private void UpdateProperty<T>(ref T field, T value, FieldInvalidationTypes invalidatedValues) where T : struct
        {
            if (field.Equals(value))
                return;
            field = value;
            _invalidatedValues = _invalidatedValues | invalidatedValues;
            if (IsDeferringChanges)
                return;
            CommitChanges();
        }

        protected override ushort CreateWindowClassCore()
        {
            return SharedWindowClassAtom;
        }

        protected override void DestroyWindowClassCore()
        {
        }

        protected override IntPtr CreateWindowCore()
        {
            return User32.CreateWindowEx(524416, new IntPtr(GetWindowClassAtom()), string.Empty, -2046820352, 0,
                0, 0, 0, new WindowInteropHelper(_targetWindow).Owner, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        }

        public void ChangeOwner(IntPtr newOwner)
        {
            NativeMethods.SetWindowLongPtrGwlp(Handle, Gwlp.Hwndparent, newOwner);
        }

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case 126:
                    if (IsVisible)
                        RenderLayeredWindow();
                    break;
                case 132:
                    return new IntPtr(WmNcHitTest(lParam));
                case 161:
                case 163:
                case 164:
                case 166:
                case 167:
                case 169:
                case 171:
                case 173:
                    var targetWindowHandle = TargetWindowHandle;
                    User32.SendMessage(targetWindowHandle, 6, new IntPtr(2), IntPtr.Zero);
                    User32.SendMessage(targetWindowHandle, msg, wParam, IntPtr.Zero);
                    return IntPtr.Zero;
                case 6:
                    return IntPtr.Zero;
                case 70:
                    var structure = (Windowpos) Marshal.PtrToStructure(lParam, typeof(Windowpos));
                    structure.flags |= 16U;
                    Marshal.StructureToPtr((object) structure, lParam, true);
                    break;
            }
            return base.WndProc(hwnd, msg, wParam, lParam);
        }

        private int WmNcHitTest(IntPtr lParam)
        {
            var xlParam = NativeMethods.GetXlParam(lParam.ToInt32());
            var ylParam = NativeMethods.GetYlParam(lParam.ToInt32());
            RECT lpRect;
            User32.GetWindowRect(Handle, out lpRect);
            switch (_orientation)
            {
                case Dock.Left:
                    if (ylParam - 18 < lpRect.Top)
                        return 13;
                    return ylParam + 18 > lpRect.Bottom ? 16 : 10;
                case Dock.Top:
                    if (xlParam - 18 < lpRect.Left)
                        return 13;
                    return xlParam + 18 > lpRect.Right ? 14 : 12;
                case Dock.Right:
                    if (ylParam - 18 < lpRect.Top)
                        return 14;
                    return ylParam + 18 > lpRect.Bottom ? 17 : 11;
                default:
                    if (xlParam - 18 < lpRect.Left)
                        return 16;
                    return xlParam + 18 > lpRect.Right ? 17 : 15;
            }
        }

        public void CommitChanges()
        {
            InvalidateCachedBitmaps();
            UpdateWindowPosCore();
            UpdateLayeredWindowCore();
            _invalidatedValues = FieldInvalidationTypes.None;
        }

        private bool InvalidatedValuesHasFlag(FieldInvalidationTypes flag)
        {
            return (uint) (_invalidatedValues & flag) > 0U;
        }

        private void InvalidateCachedBitmaps()
        {
            if (InvalidatedValuesHasFlag(FieldInvalidationTypes.ActiveColor))
                ClearCache(_activeGlowBitmaps);
            if (!InvalidatedValuesHasFlag(FieldInvalidationTypes.InactiveColor))
                return;
            ClearCache(_inactiveGlowBitmaps);
        }

        private void UpdateWindowPosCore()
        {
            if (!InvalidatedValuesHasFlag(FieldInvalidationTypes.Location | FieldInvalidationTypes.Size |
                                          FieldInvalidationTypes.Visibility))
                return;
            var flags = 532;
            if (InvalidatedValuesHasFlag(FieldInvalidationTypes.Visibility))
                if (IsVisible)
                    flags |= 64;
                else
                    flags |= 131;
            if (!InvalidatedValuesHasFlag(FieldInvalidationTypes.Location))
                flags |= 2;
            if (!InvalidatedValuesHasFlag(FieldInvalidationTypes.Size))
                flags |= 1;
            User32.SetWindowPos(Handle, IntPtr.Zero, Left, Top, Width, Height, flags);
        }

        private void UpdateLayeredWindowCore()
        {
            if (!IsVisible || !InvalidatedValuesHasFlag(FieldInvalidationTypes.Render))
                return;
            if (IsPositionValid)
            {
                BeginDelayedRender();
            }
            else
            {
                CancelDelayedRender();
                RenderLayeredWindow();
            }
        }

        private void BeginDelayedRender()
        {
            if (_pendingDelayRender)
                return;
            _pendingDelayRender = true;
            CompositionTarget.Rendering += CommitDelayedRender;
        }

        private void CancelDelayedRender()
        {
            if (!_pendingDelayRender)
                return;
            _pendingDelayRender = false;
            CompositionTarget.Rendering -= CommitDelayedRender;
        }

        private void CommitDelayedRender(object sender, EventArgs e)
        {
            CancelDelayedRender();
            if (!IsVisible)
                return;
            RenderLayeredWindow();
        }

        private void RenderLayeredWindow()
        {
            using (var drawingContext = new GlowDrawingContext(Width, Height))
            {
                if (!drawingContext.IsInitialized)
                    return;
                switch (_orientation)
                {
                    case Dock.Left:
                        DrawLeft(drawingContext);
                        break;
                    case Dock.Top:
                        DrawTop(drawingContext);
                        break;
                    case Dock.Right:
                        DrawRight(drawingContext);
                        break;
                    default:
                        DrawBottom(drawingContext);
                        break;
                }
                var pptDest = new Native.Platform.Structs.Point
                {
                    X = Left,
                    Y = Top
                };
                var psize = new Win32Size
                {
                    Cx = Width,
                    Cy = Height
                };
                var pptSrc = new Native.Platform.Structs.Point {X = 0, Y = 0};
                User32.UpdateLayeredWindow(Handle, drawingContext.ScreenDc, ref pptDest, ref psize,
                    drawingContext.WindowDc, ref pptSrc, 0U, ref drawingContext.Blend, 2U);
            }
        }

        private GlowBitmap GetOrCreateBitmap(GlowDrawingContext drawingContext, GlowBitmapPart bitmapPart)
        {
            GlowBitmap[] glowBitmapArray;
            Color color;
            if (IsActive)
            {
                glowBitmapArray = _activeGlowBitmaps;
                color = ActiveGlowColor;
            }
            else
            {
                glowBitmapArray = _inactiveGlowBitmaps;
                color = InactiveGlowColor;
            }
            var index = (int) bitmapPart;
            return glowBitmapArray[index] ?? (glowBitmapArray[index] =
                       GlowBitmap.Create(drawingContext, bitmapPart, color));
        }

        private void ClearCache(GlowBitmap[] cache)
        {
            for (var index = 0; index < cache.Length; ++index)
                using (cache[index])
                {
                    cache[index] = null;
                }
        }

        protected override void DisposeManagedResources()
        {
            ClearCache(_activeGlowBitmaps);
            ClearCache(_inactiveGlowBitmaps);
        }

        protected override void DisposeNativeResources()
        {
            base.DisposeNativeResources();
            ++_disposedGlowWindows;
        }

        private void DrawLeft(GlowDrawingContext drawingContext)
        {
            var bitmap1 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.CornerTopLeft);
            var bitmap2 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.LeftTop);
            var bitmap3 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.Left);
            var bitmap4 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.LeftBottom);
            var bitmap5 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.CornerBottomLeft);
            var height = bitmap1.Height;
            var yoriginDest1 = height + bitmap2.Height;
            var yoriginDest2 = drawingContext.Height - bitmap5.Height;
            var yoriginDest3 = yoriginDest2 - bitmap4.Height;
            var hDest = yoriginDest3 - yoriginDest1;
            Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap1.Handle);
            Msimg32.AlphaBlend(drawingContext.WindowDc, 0, 0, bitmap1.Width, bitmap1.Height,
                drawingContext.BackgroundDc, 0, 0, bitmap1.Width, bitmap1.Height, drawingContext.Blend);
            Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap2.Handle);
            Msimg32.AlphaBlend(drawingContext.WindowDc, 0, height, bitmap2.Width, bitmap2.Height,
                drawingContext.BackgroundDc, 0, 0, bitmap2.Width, bitmap2.Height, drawingContext.Blend);
            if (hDest > 0)
            {
                Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap3.Handle);
                Msimg32.AlphaBlend(drawingContext.WindowDc, 0, yoriginDest1, bitmap3.Width, hDest,
                    drawingContext.BackgroundDc, 0, 0, bitmap3.Width, bitmap3.Height, drawingContext.Blend);
            }
            Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap4.Handle);
            Msimg32.AlphaBlend(drawingContext.WindowDc, 0, yoriginDest3, bitmap4.Width, bitmap4.Height,
                drawingContext.BackgroundDc, 0, 0, bitmap4.Width, bitmap4.Height, drawingContext.Blend);
            Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap5.Handle);
            Msimg32.AlphaBlend(drawingContext.WindowDc, 0, yoriginDest2, bitmap5.Width, bitmap5.Height,
                drawingContext.BackgroundDc, 0, 0, bitmap5.Width, bitmap5.Height, drawingContext.Blend);
        }

        private void DrawRight(GlowDrawingContext drawingContext)
        {
            var bitmap1 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.CornerTopRight);
            var bitmap2 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.RightTop);
            var bitmap3 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.Right);
            var bitmap4 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.RightBottom);
            var bitmap5 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.CornerBottomRight);
            var height = bitmap1.Height;
            var yoriginDest1 = height + bitmap2.Height;
            var yoriginDest2 = drawingContext.Height - bitmap5.Height;
            var yoriginDest3 = yoriginDest2 - bitmap4.Height;
            var hDest = yoriginDest3 - yoriginDest1;
            Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap1.Handle);
            Msimg32.AlphaBlend(drawingContext.WindowDc, 0, 0, bitmap1.Width, bitmap1.Height,
                drawingContext.BackgroundDc, 0, 0, bitmap1.Width, bitmap1.Height, drawingContext.Blend);
            Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap2.Handle);
            Msimg32.AlphaBlend(drawingContext.WindowDc, 0, height, bitmap2.Width, bitmap2.Height,
                drawingContext.BackgroundDc, 0, 0, bitmap2.Width, bitmap2.Height, drawingContext.Blend);
            if (hDest > 0)
            {
                Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap3.Handle);
                Msimg32.AlphaBlend(drawingContext.WindowDc, 0, yoriginDest1, bitmap3.Width, hDest,
                    drawingContext.BackgroundDc, 0, 0, bitmap3.Width, bitmap3.Height, drawingContext.Blend);
            }
            Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap4.Handle);
            Msimg32.AlphaBlend(drawingContext.WindowDc, 0, yoriginDest3, bitmap4.Width, bitmap4.Height,
                drawingContext.BackgroundDc, 0, 0, bitmap4.Width, bitmap4.Height, drawingContext.Blend);
            Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap5.Handle);
            Msimg32.AlphaBlend(drawingContext.WindowDc, 0, yoriginDest2, bitmap5.Width, bitmap5.Height,
                drawingContext.BackgroundDc, 0, 0, bitmap5.Width, bitmap5.Height, drawingContext.Blend);
        }

        private void DrawTop(GlowDrawingContext drawingContext)
        {
            var bitmap1 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.TopLeft);
            var bitmap2 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.Top);
            var bitmap3 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.TopRight);
            var xoriginDest1 = 9;
            var xoriginDest2 = xoriginDest1 + bitmap1.Width;
            var xoriginDest3 = drawingContext.Width - 9 - bitmap3.Width;
            var wDest = xoriginDest3 - xoriginDest2;
            Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap1.Handle);
            Msimg32.AlphaBlend(drawingContext.WindowDc, xoriginDest1, 0, bitmap1.Width, bitmap1.Height,
                drawingContext.BackgroundDc, 0, 0, bitmap1.Width, bitmap1.Height, drawingContext.Blend);
            if (wDest > 0)
            {
                Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap2.Handle);
                Msimg32.AlphaBlend(drawingContext.WindowDc, xoriginDest2, 0, wDest, bitmap2.Height,
                    drawingContext.BackgroundDc, 0, 0, bitmap2.Width, bitmap2.Height, drawingContext.Blend);
            }
            Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap3.Handle);
            Msimg32.AlphaBlend(drawingContext.WindowDc, xoriginDest3, 0, bitmap3.Width, bitmap3.Height,
                drawingContext.BackgroundDc, 0, 0, bitmap3.Width, bitmap3.Height, drawingContext.Blend);
        }

        private void DrawBottom(GlowDrawingContext drawingContext)
        {
            var bitmap1 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.BottomLeft);
            var bitmap2 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.Bottom);
            var bitmap3 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.BottomRight);
            var xoriginDest1 = 9;
            var xoriginDest2 = xoriginDest1 + bitmap1.Width;
            var xoriginDest3 = drawingContext.Width - 9 - bitmap3.Width;
            var wDest = xoriginDest3 - xoriginDest2;
            Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap1.Handle);
            Msimg32.AlphaBlend(drawingContext.WindowDc, xoriginDest1, 0, bitmap1.Width, bitmap1.Height,
                drawingContext.BackgroundDc, 0, 0, bitmap1.Width, bitmap1.Height, drawingContext.Blend);
            if (wDest > 0)
            {
                Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap2.Handle);
                Msimg32.AlphaBlend(drawingContext.WindowDc, xoriginDest2, 0, wDest, bitmap2.Height,
                    drawingContext.BackgroundDc, 0, 0, bitmap2.Width, bitmap2.Height, drawingContext.Blend);
            }
            Gdi32.SelectObject(drawingContext.BackgroundDc, bitmap3.Handle);
            Msimg32.AlphaBlend(drawingContext.WindowDc, xoriginDest3, 0, bitmap3.Width, bitmap3.Height,
                drawingContext.BackgroundDc, 0, 0, bitmap3.Width, bitmap3.Height, drawingContext.Blend);
        }

        public void UpdateWindowPos()
        {
            var targetWindowHandle = TargetWindowHandle;
            RECT lpRect;
            User32.GetWindowRect(targetWindowHandle, out lpRect);
            NativeMethods.GetWindowPlacement(targetWindowHandle);
            if (!IsVisible)
                return;
            switch (_orientation)
            {
                case Dock.Left:
                    Left = lpRect.Left - 9;
                    Top = lpRect.Top - 9;
                    Width = 9;
                    Height = lpRect.Height + 18;
                    break;
                case Dock.Top:
                    Left = lpRect.Left - 9;
                    Top = lpRect.Top - 9;
                    Width = lpRect.Width + 18;
                    Height = 9;
                    break;
                case Dock.Right:
                    Left = lpRect.Right;
                    Top = lpRect.Top - 9;
                    Width = 9;
                    Height = lpRect.Height + 18;
                    break;
                default:
                    Left = lpRect.Left - 9;
                    Top = lpRect.Bottom;
                    Width = lpRect.Width + 18;
                    Height = 9;
                    break;
            }
        }

        [Flags]
        private enum FieldInvalidationTypes
        {
            None = 0,
            Location = 1,
            Size = 2,
            ActiveColor = 4,
            InactiveColor = 8,
            Render = 16,
            Visibility = 32
        }
    }

    internal sealed class GlowBitmap : DisposableObject
    {
        public const int GlowBitmapPartCount = 16;
        private static readonly CachedBitmapInfo[] TransparencyMasks = new CachedBitmapInfo[16];
        private readonly BitmapInfo _bitmapInfo;
        private readonly IntPtr _pbits;

        public GlowBitmap(IntPtr hdcScreen, int width, int height)
        {
            _bitmapInfo.BiSize = Marshal.SizeOf(typeof(Bitmapinfoheader));
            _bitmapInfo.BiPlanes = 1;
            _bitmapInfo.BiBitCount = 32;
            _bitmapInfo.BiCompression = 0;
            _bitmapInfo.BiXPelsPerMeter = 0;
            _bitmapInfo.BiYPelsPerMeter = 0;
            _bitmapInfo.BiWidth = width;
            _bitmapInfo.BiHeight = -height;
            Handle = Gdi32.CreateDIBSection(hdcScreen, ref _bitmapInfo, 0U, out _pbits, IntPtr.Zero, 0U);
        }

        public IntPtr Handle { get; }

        public IntPtr DiBits => _pbits;

        public int Width => _bitmapInfo.BiWidth;

        public int Height => -_bitmapInfo.BiHeight;

        protected override void DisposeNativeResources()
        {
            Gdi32.DeleteObject(Handle);
        }

        private static byte PremultiplyAlpha(byte channel, byte alpha)
        {
            return (byte) (channel * alpha / (double) byte.MaxValue);
        }

        public static GlowBitmap Create(GlowDrawingContext drawingContext, GlowBitmapPart bitmapPart, Color color)
        {
            var alphaMask = GetOrCreateAlphaMask(bitmapPart);
            var glowBitmap = new GlowBitmap(drawingContext.ScreenDc, alphaMask.Width, alphaMask.Height);
            var ofs = 0;
            while (ofs < alphaMask.DiBits.Length)
            {
                var diBit = alphaMask.DiBits[ofs + 3];
                var val1 = PremultiplyAlpha(color.R, diBit);
                var val2 = PremultiplyAlpha(color.G, diBit);
                var val3 = PremultiplyAlpha(color.B, diBit);
                Marshal.WriteByte(glowBitmap.DiBits, ofs, val3);
                Marshal.WriteByte(glowBitmap.DiBits, ofs + 1, val2);
                Marshal.WriteByte(glowBitmap.DiBits, ofs + 2, val1);
                Marshal.WriteByte(glowBitmap.DiBits, ofs + 3, diBit);
                ofs += 4;
            }
            return glowBitmap;
        }

        private static CachedBitmapInfo GetOrCreateAlphaMask(GlowBitmapPart bitmapPart)
        {
            var index = (int) bitmapPart;
            if (TransparencyMasks[index] != null)
                return TransparencyMasks[index];
            var bitmapImage = new BitmapImage(
                CommonUtilities.MakePackUri(typeof(GlowBitmap).Assembly, "Resources/" + bitmapPart + ".png"));
            var diBits = new byte[4 * bitmapImage.PixelWidth * bitmapImage.PixelHeight];
            var stride = 4 * bitmapImage.PixelWidth;
            bitmapImage.CopyPixels(diBits, stride, 0);
            TransparencyMasks[index] = new CachedBitmapInfo(diBits, bitmapImage.PixelWidth, bitmapImage.PixelHeight);
            return TransparencyMasks[index];
        }

        private sealed class CachedBitmapInfo
        {
            public readonly byte[] DiBits;
            public readonly int Height;
            public readonly int Width;

            public CachedBitmapInfo(byte[] diBits, int width, int height)
            {
                Width = width;
                Height = height;
                DiBits = diBits;
            }
        }
    }

    internal enum GlowBitmapPart
    {
        CornerTopLeft,
        CornerTopRight,
        CornerBottomLeft,
        CornerBottomRight,
        TopLeft,
        Top,
        TopRight,
        LeftTop,
        Left,
        LeftBottom,
        BottomLeft,
        Bottom,
        BottomRight,
        RightTop,
        Right,
        RightBottom
    }

    internal sealed class GlowDrawingContext : DisposableObject
    {
        private readonly GlowBitmap _windowBitmap;
        public BlendFunction Blend;

        public GlowDrawingContext(int width, int height)
        {
            ScreenDc = User32.GetDC(IntPtr.Zero);
            if (ScreenDc == IntPtr.Zero)
                return;
            WindowDc = Gdi32.CreateCompatibleDC(ScreenDc);
            if (WindowDc == IntPtr.Zero)
                return;
            BackgroundDc = Gdi32.CreateCompatibleDC(ScreenDc);
            if (BackgroundDc == IntPtr.Zero)
                return;
            Blend.BlendOp = 0;
            Blend.BlendFlags = 0;
            Blend.SourceConstantAlpha = byte.MaxValue;
            Blend.AlphaFormat = 1;
            _windowBitmap = new GlowBitmap(ScreenDc, width, height);
            Gdi32.SelectObject(WindowDc, _windowBitmap.Handle);
        }

        public bool IsInitialized
        {
            get
            {
                if (ScreenDc != IntPtr.Zero && WindowDc != IntPtr.Zero && BackgroundDc != IntPtr.Zero)
                    return _windowBitmap != null;
                return false;
            }
        }

        public IntPtr ScreenDc { get; }

        public IntPtr WindowDc { get; }

        public IntPtr BackgroundDc { get; }

        public int Width => _windowBitmap.Width;

        public int Height => _windowBitmap.Height;

        protected override void DisposeManagedResources()
        {
            _windowBitmap.Dispose();
        }

        protected override void DisposeNativeResources()
        {
            if (ScreenDc != IntPtr.Zero)
                User32.ReleaseDC(IntPtr.Zero, ScreenDc);
            if (WindowDc != IntPtr.Zero)
                Gdi32.DeleteDC(WindowDc);
            if (!(BackgroundDc != IntPtr.Zero))
                return;
            Gdi32.DeleteDC(BackgroundDc);
        }
    }

    internal class ChangeScope : DisposableObject
    {
        private readonly ModernChromeWindow _window;

        public ChangeScope(ModernChromeWindow window)
        {
            _window = window;
            ++_window.DeferGlowChangesCount;
        }

        protected override void DisposeManagedResources()
        {
            --_window.DeferGlowChangesCount;
            if (_window.DeferGlowChangesCount != 0)
                return;
            _window.EndDeferGlowChanges();
        }
    }
}