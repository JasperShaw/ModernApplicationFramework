using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.Core.Platform;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;
using Point = System.Windows.Point;
using DpiHelper = ModernApplicationFramework.Core.Utilities.DpiHelper;
using NativeMethods = ModernApplicationFramework.Core.NativeMethods.NativeMethods;
using RECT = ModernApplicationFramework.Core.Platform.RECT;

namespace ModernApplicationFramework.Controls
{
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    public class ModernChromeWindow : Window, IOnThemeChanged
    {
        private const int MonitorDefaulttonearest = 0x00000002;
        private readonly ShadowWindow[] _shadowWindows = new ShadowWindow[4];
        private bool _isShadowVisible;
        private int _lastWindowPlacement;
        private Rect _logicalSizeForRestore = Rect.Empty;
        private DispatcherTimer _makeShadowVisibleTimer;
        private IntPtr _ownerForActivate;
        private bool _updatingZOrder;
        private bool _useLogicalSizeForRestore;

        private int _lastScwParam;
        private WindowState _lastState;
        private bool _wasMaximized;

        protected enum ClipRegionChangeType
        {
            FromSize,
            FromPosition,
            FromPropertyChange,
            FromUndockSingleTab
        }

        public virtual void OnThemeChanged(Theme oldValue, Theme newValue)
        {
            UpdateGlowColors();
            IsShadowVisible = false;
            IsShadowVisible = true;
            UpdateClipRegion();
        }

        public Brush ActiveShadowColor
        {
            get { return (Brush) GetValue(ActiveShadowColorProperty); }
            set { SetValue(ActiveShadowColorProperty, value); }
        }

        public int CornerRadius
        {
            get { return (int) GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Brush InactiveShadowColor
        {
            get { return (Brush) GetValue(InactiveShadowColorProperty); }
            set { SetValue(InactiveShadowColorProperty, value); }
        }

        public Brush NonClientFillColor
        {
            get { return (Brush) GetValue(NonClientFillColorProperty); }
            set { SetValue(NonClientFillColorProperty, value); }
        }

        protected virtual bool ShouldShowBorder
        {
            get
            {
                var handle = new WindowInteropHelper(this).Handle;
                if (NativeMethods.IsWindowVisible(handle) && !NativeMethods.IsIconic(handle) &&
                    !NativeMethods.IsZoomed(handle))
                {
	                return ResizeMode != ResizeMode.NoResize;
                }
	            return false;
            }
        }

        protected virtual bool ShouldShowShadow
        {
            get
            {
                var handle = new WindowInteropHelper(this).Handle;
                if (NativeMethods.IsWindowVisible(handle) && !NativeMethods.IsIconic(handle) &&
                    !NativeMethods.IsZoomed(handle))
                {
	                if (ResizeMode == ResizeMode.NoResize)
                        return true;
	                return true;
                }
	            return false;
            }
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

        private IEnumerable<ShadowWindow> LoadedShadowWindows
        {
            get { return _shadowWindows.Where(w => w != null); }
        }

        private bool IsShadowVisible
        {
            get { return _isShadowVisible; }
            set
            {
                if (_isShadowVisible == value)
                    return;
                _isShadowVisible = value;
                for (var direction = 0; direction < _shadowWindows.Length; ++direction)
                    GetOrCreateShadowWindow(direction).IsVisible = value;
            }
        }

        public static void ShowWindowMenu(HwndSource source, Visual element, Point elementPoint,
            Size elementSize)
        {
            if (elementPoint.X < 0.0 || elementPoint.X > elementSize.Width ||
                (elementPoint.Y < 0.0 || elementPoint.Y > elementSize.Height))
                return;
            Point screenPoint = element.PointToScreen(elementPoint);
            ShowWindowMenu(source, screenPoint, true);
        }

        public void ChangeOwner(IntPtr newOwner)
        {
            new WindowInteropHelper(this).Owner = newOwner;
            foreach (var shadowWindow in LoadedShadowWindows)
                shadowWindow.ChangeOwner(newOwner);
            UpdateZOrderOfThisAndOwner();
        }

        public void ChangeOwnerForActivate(IntPtr newOwner)
        {
            _ownerForActivate = newOwner;
        }

        protected static void ShowWindowMenu(HwndSource source, Point screenPoint, bool canMinimize)
        {
            var systemMetrics = NativeMethods.GetSystemMetrics(40);
            var systemMenu = NativeMethods.GetSystemMenu(source.Handle, false);
            var windowPlacement = NativeMethods.GetWindowPlacement(source.Handle);
            var flag = VisualUtilities.ModifyStyle(source.Handle, 268435456, 0);
            var uEnable = canMinimize ? 0U : 1U;
            if (windowPlacement.showCmd == 1)
            {
                NativeMethods.EnableMenuItem(systemMenu, 61728U, 1U);
                NativeMethods.EnableMenuItem(systemMenu, 61456U, 0U);
                NativeMethods.EnableMenuItem(systemMenu, 61440U, 0U);
                NativeMethods.EnableMenuItem(systemMenu, 61488U, 0U);
                NativeMethods.EnableMenuItem(systemMenu, 61472U, uEnable);
                NativeMethods.EnableMenuItem(systemMenu, 61536U, 0U);
            }
            else if (windowPlacement.showCmd == 3)
            {
                NativeMethods.EnableMenuItem(systemMenu, 61728U, 0U);
                NativeMethods.EnableMenuItem(systemMenu, 61456U, 1U);
                NativeMethods.EnableMenuItem(systemMenu, 61440U, 1U);
                NativeMethods.EnableMenuItem(systemMenu, 61488U, 1U);
                NativeMethods.EnableMenuItem(systemMenu, 61472U, uEnable);
                NativeMethods.EnableMenuItem(systemMenu, 61536U, 0U);
            }
            if (flag)
                VisualUtilities.ModifyStyle(source.Handle, 0, 268435456);
            var fuFlags = (uint) (systemMetrics | 256 | 128 | 2);
            var num = NativeMethods.TrackPopupMenuEx(systemMenu, fuFlags, (int) screenPoint.X, (int) screenPoint.Y,
                source.Handle, IntPtr.Zero);
            if (num == 0)
                return;
            NativeMethods.PostMessage(source.Handle, 274, new IntPtr(num), IntPtr.Zero);
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
            var num10 = IntPtr.Zero;
            try
            {
                num1 = ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (int) cornerRadius.TopLeft);
                num2 = ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (int) cornerRadius.TopRight);
                num3 = ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height,
                    (int) cornerRadius.BottomLeft);
                num4 = ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height,
                    (int) cornerRadius.BottomRight);
                var point = new Core.Platform.Point
                {
                    X = rect.X + rect.Width/2,
                    Y = rect.Y + rect.Height/2
                };
                num5 = NativeMethods.CreateRectRgn(rect.X, rect.Y, point.X + 1, point.Y + 1);
                num6 = NativeMethods.CreateRectRgn(point.X - 1, rect.Y, rect.X + rect.Width, point.Y + 1);
                num7 = NativeMethods.CreateRectRgn(rect.X, point.Y - 1, point.X + 1, rect.Y + rect.Height);
                num8 = NativeMethods.CreateRectRgn(point.X - 1, point.Y - 1, rect.X + rect.Width, rect.Y + rect.Height);
                num9 = NativeMethods.CreateRectRgn(0, 0, 1, 1);
                num10 = NativeMethods.CreateRectRgn(0, 0, 1, 1);
                NativeMethods.CombineRgn(num10, num1, num5, NativeMethods.CombineMode.RgnAnd);
                NativeMethods.CombineRgn(num9, num2, num6, NativeMethods.CombineMode.RgnAnd);
                NativeMethods.CombineRgn(num10, num10, num9, NativeMethods.CombineMode.RgnOr);
                NativeMethods.CombineRgn(num9, num3, num7, NativeMethods.CombineMode.RgnAnd);
                NativeMethods.CombineRgn(num10, num10, num9, NativeMethods.CombineMode.RgnOr);
                NativeMethods.CombineRgn(num9, num4, num8, NativeMethods.CombineMode.RgnAnd);
                NativeMethods.CombineRgn(num10, num10, num9, NativeMethods.CombineMode.RgnOr);
            }
            finally
            {
                if (num1 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num1);
                if (num2 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num2);
                if (num3 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num3);
                if (num4 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num4);
                if (num5 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num5);
                if (num6 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num6);
                if (num7 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num7);
                if (num8 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num8);
                if (num9 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num9);
            }
            return num10;
        }

        protected override void OnActivated(EventArgs e)
        {
            UpdateGlowActiveState();
            base.OnActivated(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            StopShadowTimer();
            DestroyShadowWindows();
            base.OnClosed(e);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            if (ResizeMode == ResizeMode.CanMinimize || ResizeMode == ResizeMode.NoResize)
            {
                foreach (var shadowWindow in LoadedShadowWindows)
                    shadowWindow.IsHitTestVisible = false;
            }

            if (IsActive && WindowState == WindowState.Normal)
            {
                UpdateGlowWindowPositions(false);
                UpdateGlowActiveState();
            }
            base.OnContentRendered(e);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            UpdateGlowActiveState();
            base.OnDeactivated(e);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            var hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            hwndSource?.AddHook(WindowProc);
            Loaded += OnLoaded;
            base.OnSourceInitialized(e);       
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowStyle = WindowStyle.None;
            if (_lastScwParam == 61728 && _lastState == WindowState.Minimized && WindowState != WindowState.Maximized)
                Thread.Sleep(200);
            Topmost = WindowState == WindowState.Minimized;
            base.OnStateChanged(e);
        }

        protected virtual void OnWindowPosChanged(IntPtr hWnd, int showCmd, Int32Rect rcNormalPosition)
        {
        }

        protected void SetRoundRect(IntPtr hWnd, int width, int height)
        {
            var roundRectRegion = ComputeRoundRectRegion(0, 0, width, height, CornerRadius);
            NativeMethods.SetWindowRgn(hWnd, roundRectRegion, NativeMethods.IsWindowVisible(hWnd));
        }

        protected void UpdateClipRegion(ClipRegionChangeType regionChangeType = ClipRegionChangeType.FromPropertyChange)
        {
            var hwndSource = (HwndSource) PresentationSource.FromVisual(this);
            if (hwndSource == null)
                return;
            RECT lpRect;
            NativeMethods.GetWindowRect(hwndSource.Handle, out lpRect);
            var windowPlacement = NativeMethods.GetWindowPlacement(hwndSource.Handle);
            UpdateClipRegion(hwndSource.Handle, windowPlacement, regionChangeType, lpRect);
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

        protected virtual IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            switch (msg)
            {
                case 174:
                case 175:
                    break;
                case 274: // SysCommand
                    WmSysCommand(hwnd, wparam);
                    break;
                case 128: // SetIcon
                case 12: // SetTitle
                    CallDefWindowProcWithoutVisibleStyle(hwnd, ref handled);
                    break;
                case 132: // HitTest
                    return WmHcHitTest(lparam, ref handled);
                case 133: // Paint
                    return WmNcPaint(hwnd, wparam, ref handled);
                case 134: // NCActivate
                    return WmNcActivate(hwnd, wparam, ref handled);
                case 164:
                case 165:
                case 166: // ButtonClicks
                    RaiseNonClientMouseMessageAsClient(hwnd, msg, lparam);
                    handled = true;
                    break;
                case 6:
                    WmActivate(wparam, lparam);
                    break;
                case 70:
                    WmWindowPosChanging(lparam);
                    break;
				case 71:
                    WmWindowPosChanged(hwnd, lparam);
                    break;
                case 36: // GetMinMaxInfo
                    WnGetMinMaxInfo(hwnd, lparam);
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        private static Minmaxinfo AdjustWorkingAreaForAutoHide(IntPtr monitor, Minmaxinfo mmi)
        {
            var hwnd = NativeMethods.FindWindow("Shell_TrayWnd", null);
            var monitorWithTaskBar = NativeMethods.MonitorFromWindow(hwnd, MonitorDefaulttonearest);
            if (!monitor.Equals(monitorWithTaskBar))
                return mmi;
            var abd = new Appbardata();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = hwnd;
            NativeMethods.SHAppBarMessage((int) AbMsg.AbmGettaskbarpos, ref abd);
            var uEdge = GetEdge(abd.rc);
            var autoHide = Convert.ToBoolean(NativeMethods.SHAppBarMessage((int) AbMsg.AbmGetstate, ref abd));

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

        private static void CallDefWindowProcWithoutVisibleStyle(IntPtr hwnd, ref bool handled)
        {
            var flag = VisualUtilities.ModifyStyle(hwnd, 268435456, 0);
            if (flag)
                VisualUtilities.ModifyStyle(hwnd, 0, 268435456);
            handled = true;
        }

        private static void ClearClipRegion(IntPtr hWnd)
        {
            NativeMethods.SetWindowRgn(hWnd, IntPtr.Zero, NativeMethods.IsWindowVisible(hWnd));
        }

        private static IntPtr ComputeRoundRectRegion(int left, int top, int width, int height, int cornerRadius)
        {
            var nWidthEllipse = (int) (2*cornerRadius*DpiHelper.LogicalToDeviceUnitsScalingFactorX);
            var nHeightEllipse = (int) (2*cornerRadius*DpiHelper.LogicalToDeviceUnitsScalingFactorY);
            return NativeMethods.CreateRoundRectRgn(left, top, left + width + 1, top + height + 1, nWidthEllipse,
                nHeightEllipse);
        }

        private static RECT GetClientRectRelativeToWindowRect(IntPtr hwnd)
        {
            RECT lpRect1;
            NativeMethods.GetWindowRect(hwnd, out lpRect1);
            RECT lpRect2;
            NativeMethods.GetClientRect(hwnd, out lpRect2);
            var point = new Core.Platform.Point {X = 0, Y = 0};
            NativeMethods.ClientToScreen(hwnd, ref point);
            lpRect2.Offset(point.X - lpRect1.Left, point.Y - lpRect1.Top);
            return lpRect2;
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

        private static Monitorinfo MonitorInfoFromWindow(IntPtr hWnd)
        {
            var hMonitor = NativeMethods.MonitorFromWindow(hWnd, 2);
            var monitorInfo = new Monitorinfo { CbSize = (uint)Marshal.SizeOf(typeof(Monitorinfo)) };
            NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo);
            return monitorInfo;
        }

        private static void OnCornerRadiusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((ModernChromeWindow) obj).UpdateClipRegion();
        }

        private static void OnGlowColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ModernChromeWindow) d).UpdateGlowColors();
        }

        private static void RaiseNonClientMouseMessageAsClient(IntPtr hwnd, int msg, IntPtr lparam)
        {
            var point = new Core.Platform.Point
            {
                X = NativeMethods.GetXlParam(lparam.ToInt32()),
                Y = NativeMethods.GetYlParam(lparam.ToInt32())
            };
            NativeMethods.ScreenToClient(hwnd, ref point);
            NativeMethods.SendMessage(hwnd, msg + 513 - 161, new IntPtr(PressedMouseButtons),
                NativeMethods.MakeParam(point.X, point.Y));
        }

        private static void UpdateMaximizedClipRegion(IntPtr hWnd)
        {
            var relativeToWindowRect = GetClientRectRelativeToWindowRect(hWnd);
            ++relativeToWindowRect.Bottom;
            var rectRgnIndirect = NativeMethods.CreateRectRgnIndirect(ref relativeToWindowRect);
            NativeMethods.SetWindowRgn(hWnd, rectRgnIndirect, NativeMethods.IsWindowVisible(hWnd));
        }

        private static void UpdateZOrderOfOwner(IntPtr hwndOwner)
        {
            var lastOwnedWindow = IntPtr.Zero;
            NativeMethods.EnumThreadWindows(NativeMethods.GetCurrentThreadId(), (hwnd, lParam) =>
            {
                if (NativeMethods.GetWindow(hwnd, 4) == hwndOwner)
                    lastOwnedWindow = hwnd;
                return true;
            }, IntPtr.Zero);
            if (!(lastOwnedWindow != IntPtr.Zero) || !(NativeMethods.GetWindow(hwndOwner, 3) != lastOwnedWindow))
                return;
            NativeMethods.SetWindowPos(hwndOwner, lastOwnedWindow, 0, 0, 0, 0, 19);
        }

        private static IntPtr WmNcActivate(IntPtr hWnd, IntPtr wParam, ref bool handled)
        {
            handled = true;
            return NativeMethods.DefWindowProc(hWnd, 134, wParam, new IntPtr(-1));
        }

        private static IntPtr WmNcPaint(IntPtr hwnd, IntPtr wparam, ref bool handled)
        {
            var hrgnClip = wparam == new IntPtr(1) ? IntPtr.Zero : wparam;
            var dcEx = NativeMethods.GetDCEx(hwnd, hrgnClip, 155);
            if (dcEx != IntPtr.Zero)
            {
                try
                {
                    var nonClientFillColor = Colors.Black;
                    var solidBrush =
                        NativeMethods.CreateSolidBrush(nonClientFillColor.B << 16 | nonClientFillColor.G << 8 |
                                                       nonClientFillColor.R);
                    try
                    {
                        var relativeToWindowRect = GetClientRectRelativeToWindowRect(hwnd);
                        relativeToWindowRect.Top = relativeToWindowRect.Bottom;
                        relativeToWindowRect.Bottom = relativeToWindowRect.Top + 1;
                        NativeMethods.FillRect(dcEx, ref relativeToWindowRect, solidBrush);
                    }
                    finally
                    {
                        NativeMethods.DeleteObject(solidBrush);
                    }
                }
                finally
                {
                    NativeMethods.ReleaseDC(hwnd, dcEx);
                }
            }
            handled = true;
            return IntPtr.Zero;
        }

        private void CreateShadowWindowHandles()
        {
            for (var direction = 0; direction < _shadowWindows.Length; ++direction)
                GetOrCreateShadowWindow(direction);
        }

        private void DestroyShadowWindows()
        {
            for (var index = 0; index < _shadowWindows.Length; ++index)
                using (_shadowWindows[index])
                    _shadowWindows[index] = null;
        }

        private ShadowWindow GetOrCreateShadowWindow(int direction)
        {
            return _shadowWindows[direction] ??
                   (_shadowWindows[direction] = new ShadowWindow(this, (Dock) direction));
        }

        private bool IsAeroSnappedToMonitor(IntPtr hwnd)
        {
            var monitorInfo = MonitorInfoFromWindow(hwnd);
            var rect = new Rect(Left, Top, Width, Height).LogicalToDeviceUnits();
            return Math.Abs(monitorInfo.RcWork.Height - rect.Height) < 0 &&
                   Math.Abs(monitorInfo.RcWork.Top - rect.Top) < 0;
        }

        private void OnDelayedShadowVisibilityTimerTick(object sender, EventArgs e)
        {
            StopShadowTimer();
            UpdateGlowWindowPositions(false);
            UpdateClipRegion();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
			CreateShadowWindowHandles();
        }

        private void StopShadowTimer()
        {
            if (_makeShadowVisibleTimer == null)
                return;
            _makeShadowVisibleTimer.Stop();
            _makeShadowVisibleTimer.Tick -= OnDelayedShadowVisibilityTimerTick;
            _makeShadowVisibleTimer = null;
        }

        private void UpdateClipRegion(IntPtr hWnd, Windowplacement placement, ClipRegionChangeType changeType,
            RECT currentBounds)
        {
            UpdateClipRegionCore(hWnd, placement.showCmd, changeType, currentBounds.ToInt32Rect());
            _lastWindowPlacement = placement.showCmd;
        }

        private void UpdateGlowActiveState()
        {
            foreach (var shadowWindow in LoadedShadowWindows)
                shadowWindow.IsActive = IsActive;
        }

        private void UpdateGlowColors()
        {
            foreach (var shadowWindow in LoadedShadowWindows)
            {
                shadowWindow.ActiveBorderBrush = ActiveShadowColor;
                shadowWindow.InactiveBorderBrush = InactiveShadowColor;
            }
        }

        private void UpdateGlowVisibility(bool delayIfNecessary)
        {
            var shouldShowShadow = ShouldShowShadow;
            if (shouldShowShadow == IsShadowVisible)
                return;
            if (SystemParameters.MinimizeAnimation & shouldShowShadow & delayIfNecessary)
            {
                if (_makeShadowVisibleTimer != null)
                    _makeShadowVisibleTimer.Stop();
                else
                {
                    _makeShadowVisibleTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(200.0)};
                    _makeShadowVisibleTimer.Tick += OnDelayedShadowVisibilityTimerTick;
                }
                _makeShadowVisibleTimer.Start();
            }
            else
            {
                StopShadowTimer();
                IsShadowVisible = shouldShowShadow;
            }
        }

        private void UpdateGlowWindowPositions(bool delayIfNecessary)
        {
            UpdateGlowVisibility(delayIfNecessary);
            foreach (var shadowWindow in LoadedShadowWindows)
                shadowWindow.UpdateWindowPos();
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
                foreach (var shadowWindow in LoadedShadowWindows)
                {
                    if (NativeMethods.GetWindow(new WindowInteropHelper(shadowWindow).Handle, 3) != handle)
                        NativeMethods.SetWindowPos(new WindowInteropHelper(shadowWindow).Handle, handle, 0, 0, 0, 0, 19);
                    handle = new WindowInteropHelper(shadowWindow).Handle;
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

        private void WmActivate(IntPtr wParam, IntPtr lParam)
        {
            if (!(_ownerForActivate != IntPtr.Zero))
                return;
            NativeMethods.SendMessage(_ownerForActivate, NativeMethods.Notifyowneractivate, wParam, lParam);
        }

        private IntPtr WmHcHitTest(IntPtr lparam, ref bool handled)
        {
            if (PresentationSource.FromDependencyObject(this) == null)
                return new IntPtr(0);
            var point1 = new Point(NativeMethods.GetXlParam(lparam.ToInt32()),
                NativeMethods.GetYlParam(lparam.ToInt32()));
            var point2 = PointFromScreen(point1);
            DependencyObject visualHit = null;
            VisualUtilities.HitTestVisibleElements(this,
                target =>
                {
                    visualHit = target.VisualHit;
                    return HitTestResultBehavior.Stop;
                }, new PointHitTestParameters(point2));
            var num = 0;
            for (; visualHit != null; visualHit = visualHit.GetVisualOrLogicalParent1())
            {
                var nonClientArea = visualHit as INonClientArea;
                if (nonClientArea == null)
                    continue;
                num = nonClientArea.HitTest(point1);
                if (num != 0)
                    break;
            }
            if (num == 0)
                num = 1;
            handled = true;
            return new IntPtr(num);
        }

        private void WmSysCommand(IntPtr hwnd, IntPtr wparam)
        {
            var scWparam = (int) wparam & 65520;
            _lastScwParam = scWparam;
            _lastState = WindowState;
            if (scWparam == 61456)
            {
                NativeMethods.RedrawWindow(hwnd, IntPtr.Zero, IntPtr.Zero,
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

            if ((scWparam == 61488 || scWparam == 61472 || (scWparam == 61456 || scWparam == 61440)) &&
                (WindowState == WindowState.Normal && !IsAeroSnappedToMonitor(hwnd)))
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
        }

        private void WmWindowPosChanged(IntPtr hWnd, IntPtr lParam)
        {
            try
            {
                var windowpos = (Windowpos) Marshal.PtrToStructure(lParam, typeof (Windowpos));
                var windowPlacement = NativeMethods.GetWindowPlacement(hWnd);
                var currentBounds = new RECT(windowpos.x, windowpos.y, windowpos.x + windowpos.cx,
                    windowpos.y + windowpos.cy);
                if (((int) windowpos.flags & 1) != 1)
                    UpdateClipRegion(hWnd, windowPlacement, ClipRegionChangeType.FromSize, currentBounds);
                else if (((int) windowpos.flags & 2) != 2)
                    UpdateClipRegion(hWnd, windowPlacement, ClipRegionChangeType.FromPosition, currentBounds);
                OnWindowPosChanged(hWnd, windowPlacement.showCmd, windowPlacement.rcNormalPosition.ToInt32Rect());
                UpdateGlowWindowPositions(true);
                UpdateZOrderOfThisAndOwner();
            }
            catch (Win32Exception)
            {
            }
        }

        private void WmWindowPosChanging(IntPtr lParam)
        {
            var windowpos = (Windowpos) Marshal.PtrToStructure(lParam, typeof (Windowpos));
            if (((int) windowpos.flags & 2) != 0 || ((int) windowpos.flags & 1) != 0 ||
                (windowpos.cx <= 0 || windowpos.cy <= 0))
                return;
            var floatRect = new Rect(windowpos.x, windowpos.y, windowpos.cx, windowpos.cy).DeviceToLogicalUnits();
            if (_useLogicalSizeForRestore)
            {
                floatRect = _logicalSizeForRestore;
                _logicalSizeForRestore = Rect.Empty;
                _useLogicalSizeForRestore = false;
            }
            var rect = ViewSite.GetOnScreenPosition(floatRect).LogicalToDeviceUnits();
            windowpos.x = (int) rect.X;
            windowpos.y = (int) rect.Y;
            Marshal.StructureToPtr((object) windowpos, lParam, true);
        }

        private void WnGetMinMaxInfo(IntPtr hwnd, IntPtr lparam)
        {
            var mmi = (Minmaxinfo) Marshal.PtrToStructure(lparam, typeof (Minmaxinfo));
            var monitor = NativeMethods.MonitorFromWindow(hwnd, MonitorDefaulttonearest);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MonitorInfo();
                NativeMethods.GetMonitorInfo(monitor, monitorInfo);
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

        public static readonly DependencyProperty ActiveShadowColorProperty =
            DependencyProperty.Register("ActiveShadowColor", typeof (Brush), typeof (ModernChromeWindow),
                new FrameworkPropertyMetadata(Brushes.Black, OnGlowColorChanged));

        public static readonly DependencyProperty InactiveShadowColorProperty =
            DependencyProperty.Register("InactiveShadowColor", typeof (Brush), typeof (ModernChromeWindow),
                new FrameworkPropertyMetadata(Brushes.DarkGray, OnGlowColorChanged));

        public static readonly DependencyProperty NonClientFillColorProperty =
            DependencyProperty.Register("NonClientFillColor", typeof (Brush), typeof (ModernChromeWindow),
                new FrameworkPropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius",
            typeof (int), typeof (ModernChromeWindow), new FrameworkPropertyMetadata(0, OnCornerRadiusChanged));
    }

    [TemplatePart(Name = InnerBorder, Type = typeof (Border))]
    [TemplatePart(Name = BorderName, Type = typeof (Border))]
    internal sealed class ShadowWindow : Window, IDisposable
    {
        private const string BorderName = "PART_Border";
        private const int CornerTolerance = 18;
        private const string InnerBorder = "PART_Inner";
        internal Border Border;
        internal Border Shadow;
        private readonly Dock _direction;
        private readonly ModernChromeWindow _targetWindow;
        private bool _isActive;
        private bool _isVisible;

        public ShadowWindow(ModernChromeWindow owner, Dock direction)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;

            _targetWindow = owner;
            Owner = owner;

	        ShowInTaskbar = false;

            ActiveBorderBrush = _targetWindow.ActiveShadowColor ?? Brushes.Black;
            InactiveBorderBrush = _targetWindow.InactiveShadowColor ?? Brushes.DarkGray;

            Foreground = IsActive ? ActiveBorderBrush : InactiveBorderBrush;
            _direction = direction;

            Height = _targetWindow.Height;
            Width = _targetWindow.Width;
            Top = _targetWindow.Top;
            Left = _targetWindow.Left;
        }

        static ShadowWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ShadowWindow),
                new FrameworkPropertyMetadata(typeof (ShadowWindow)));
        }

        public void Dispose()
        {
            Close();
        }

        public Brush ActiveBorderBrush { get; set; }
        public Brush InactiveBorderBrush { get; set; }

        public new bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                CommitChange();
            }
        }

        public new bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                CommitChange();
            }
        }

        private IntPtr TargetWindowHandle => new WindowInteropHelper(_targetWindow).Handle;

        public void ChangeOwner(IntPtr newOwner)
        {
            NativeMethods.SetWindowLongPtr(new WindowInteropHelper(this).Handle, Gwlp.Hwndparent, newOwner);
        }

        public void CommitChange()
        {
            ClearWindow();
            UpdateWindowPosCore();
            UpdateWindowLayerCore();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Shadow = GetTemplateChild(BorderName) as Border;
            Border = GetTemplateChild(InnerBorder) as Border;
            if (Shadow == null || Border == null)
                return;

            switch (_direction)
            {
                case Dock.Left:
                    Shadow.Margin = new Thickness(8, 8, -8, 8);
                    Border.BorderThickness = new Thickness(0, 0, 1, 0);
                    Border.Height = _targetWindow.Height;
                    break;

                case Dock.Top:
                    Shadow.Margin = new Thickness(8, 8, 8, -8);
                    Border.BorderThickness = new Thickness(0, 0, 0, 1);
                    Border.Width = _targetWindow.Width + 2;
                    break;
                case Dock.Right:
                    Shadow.Margin = new Thickness(-8, 8, 8, 8);
                    Border.BorderThickness = new Thickness(1, 0, 0, 0);
                    Border.Height = _targetWindow.Height;
                    break;
                default:
                    Shadow.Margin = new Thickness(8, -8, 8, 8);
                    Border.BorderThickness = new Thickness(0, 1, 0, 0);
                    Border.Width = _targetWindow.Width + 2;
                    break;
            }
        }

        public void UpdateWindowPos()
        {
            var targetWindowHandle = TargetWindowHandle;
            RECT lpRect;
            NativeMethods.GetWindowRect(targetWindowHandle, out lpRect);
            NativeMethods.GetWindowPlacement(targetWindowHandle);

            if (!IsVisible)
                return;
            switch (_direction)
            {
                case Dock.Left:
                    Border.Height = lpRect.Height;
                    Left = lpRect.Left - 9;
                    Top = lpRect.Top - 9;
                    Width = 9;
                    Height = lpRect.Height + 18;
                    break;
                case Dock.Top:
                    Border.Width = lpRect.Width + 2;
                    Left = lpRect.Left - 9;
                    Top = lpRect.Top - 9;
                    Width = lpRect.Width + 18;
                    Height = 9;
                    break;
                case Dock.Right:
                    Border.Height = lpRect.Height;
                    Left = lpRect.Right;
                    Top = lpRect.Top - 9;
                    Width = 9;
                    Height = lpRect.Height + 18;
                    break;
                default:
                    Border.Width = lpRect.Width + 2;
                    Left = lpRect.Left - 9;
                    Top = lpRect.Bottom;
                    Width = lpRect.Width + 18;
                    Height = 9;
                    break;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwndSource = HwndSource.FromHwnd((new WindowInteropHelper(this).Handle));
            hwndSource?.AddHook(WndProc);

			Loaded += OnLoaded;
        }

		//Makes window not show in App-Switcher (Alt+Tab)
		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var wndHelper = new WindowInteropHelper(this);
			var exStyle = NativeMethods.GetWindowLong(wndHelper.Handle, Gwl.Exstyle);

			exStyle |= 0x00000080;

			NativeMethods.SetWindowLongShadow(wndHelper.Handle, (int)Gwl.Exstyle, (IntPtr)exStyle);

		}


		private void ClearWindow()
        {
            Hide();
        }

        private int DecideResizeDirection(Point p)
        {
            switch (_direction)
            {
                case Dock.Left:
                    if (p.Y < CornerTolerance)
                        return 13;
                    return p.Y > ActualHeight - CornerTolerance ? 16 : 10;
                case Dock.Top:
                    if (p.X < CornerTolerance)
                        return 13;
                    return p.X > ActualWidth - CornerTolerance ? 14 : 12;
                case Dock.Right:
                    if (p.Y < CornerTolerance)
                        return 14;
                    return p.Y > ActualHeight - CornerTolerance ? 17 : 11;
                default:
                    if (p.X < CornerTolerance)
                        return 16;
                    return p.X > ActualWidth - CornerTolerance ? 17 : 15;
            }
        }

        private Cursor GetCursor(IntPtr lParam)
        {
            var xlParam = NativeMethods.GetXlParam(lParam.ToInt32());
            var ylParam = NativeMethods.GetYlParam(lParam.ToInt32());
            RECT lpRect;
            NativeMethods.GetWindowRect(new WindowInteropHelper(this).Handle, out lpRect);

            switch (_direction)
            {
                case Dock.Left:
                    if (ylParam - 18 < lpRect.Top)
                        return Cursors.SizeNWSE;
                    return ylParam + 18 > lpRect.Bottom ? Cursors.SizeNESW : Cursors.SizeWE;
                case Dock.Top:
                    if (xlParam - 18 < lpRect.Left)
                        return Cursors.SizeNWSE;
                    return xlParam + 18 > lpRect.Right ? Cursors.SizeNESW : Cursors.SizeNS;
                case Dock.Right:
                    if (ylParam - 18 < lpRect.Top)
                        return Cursors.SizeNESW;
                    return ylParam + 18 > lpRect.Bottom ? Cursors.SizeNWSE : Cursors.SizeWE;
                default:
                    if (xlParam - 18 < lpRect.Left)
                        return Cursors.SizeNESW;
                    return xlParam + 18 > lpRect.Right ? Cursors.SizeNWSE : Cursors.SizeNS;
            }
        }

        private void RenderLayeredWindow()
        {
            Foreground = _isActive ? ActiveBorderBrush : InactiveBorderBrush;
            Show();
        }

        private void UpdateWindowLayerCore()
        {
            if (!IsVisible)
                return;
            RenderLayeredWindow();
        }

        private void UpdateWindowPosCore()
        {
            var flags = 532;
            if (IsVisible)
                flags |= 64;
            else
                flags |= 131;
            NativeMethods.SetWindowPos(new WindowInteropHelper(this).Handle, IntPtr.Zero, (int) Left, (int) Top,
                (int) Width, (int) Height,
                flags);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 126:
                    if (IsVisible)
                        RenderLayeredWindow();
                    break;
                case 33:
                    handled = true;
                    return new IntPtr(3);
                case 132:
                    Cursor = GetCursor(lParam) ?? Cursors.None;
                    break;
                case 161:
                case 163:
                case 164:
                case 166:
                case 167:
                case 169:
                case 171:
                case 173:
                    var targetWindowHandle = TargetWindowHandle;
                    NativeMethods.SendMessage(targetWindowHandle, 6, new IntPtr(2), IntPtr.Zero);
                    NativeMethods.SendMessage(targetWindowHandle, msg, wParam, IntPtr.Zero);
                    break;
                case 6:
                    return IntPtr.Zero;
                case 70:
                    var windowPos = (Windowpos) Marshal.PtrToStructure(lParam, typeof (Windowpos));
                    windowPos.flags |= 16U;
                    Marshal.StructureToPtr(windowPos, lParam, true);
                    break;
                case 513:
                    if (!IsHitTestVisible)
                        break;
                    var pt = new Point((int) lParam & 0xFFFF, ((int) lParam >> 16) & 0xFFFF);
                    NativeMethods.PostMessage(TargetWindowHandle,
                        161, (IntPtr) DecideResizeDirection(pt),
                        IntPtr.Zero);
                    break;
            }
            return NativeMethods.DefWindowProc(hwnd, msg, wParam, lParam);
        }
    }
}