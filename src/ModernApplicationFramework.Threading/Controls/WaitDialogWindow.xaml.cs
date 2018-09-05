using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using ModernApplicationFramework.Threading.NativeMethods;

namespace ModernApplicationFramework.Threading.Controls
{
    public partial class WaitDialogWindow
    {
        private static readonly TimeSpan RetryTimerDelay = TimeSpan.FromMilliseconds(250.0);
        private WindowInteropHelper _interopHelper;
        private IntPtr _dialogWindowHandle;
        private readonly IntPtr _hostMainWindowHandle;
        private IntPtr _hostActiveWindowHandle;
        private string _hostRootWindowCaption;
        private readonly int _hostProcessId;
        private readonly DispatcherTimer _dispatcherTimer;

        public event EventHandler Cancelled;

        public WaitDialogWindow(IntPtr hostMainWindowHandle, int hostProcessId)
        {
            InitializeComponent();
            _hostMainWindowHandle = hostMainWindowHandle;
            _hostProcessId = hostProcessId;
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
        }

        private void WaitDialogWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _interopHelper = new WindowInteropHelper(this);
            _dialogWindowHandle = _interopHelper.Handle;
            User32.SetWindowLong(_dialogWindowHandle, (int)Gwl.Style, User32.GetWindowLong(_dialogWindowHandle, (int)Gwl.Style) & -524289 | int.MinValue);
            User32.SetProp(_dialogWindowHandle, "UIA_WindowPatternEnabled", new IntPtr(1));
            User32.SetProp(_dialogWindowHandle, "UIA_WindowVisibilityOverridden", new IntPtr(1));
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Cancelled?.Invoke(this, EventArgs.Empty);
        }

        public void TryShowDialog(IntPtr hostActiveWindowHandle, string rootWindowCaption)
        {
            _hostActiveWindowHandle = hostActiveWindowHandle;
            _hostRootWindowCaption = rootWindowCaption;
            if (CanShowDialog())
                PositionAndShowDialog();
            if (_hostActiveWindowHandle != IntPtr.Zero)
            {
                _dispatcherTimer.Interval = RetryTimerDelay;
                _dispatcherTimer.Start();
            }
            else
                _dispatcherTimer.Stop();
        }

        public void HideDialog()
        {
            _dispatcherTimer.Stop();
            Hide();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!IsVisible)
                TryShowDialog(_hostActiveWindowHandle, _hostRootWindowCaption);
            else if (CanShowDialog())
            {
                User32.SetWindowPos(_dialogWindowHandle, new IntPtr(-1), 0, 0, 0, 0, 19);
            }
            else
            {
                IntPtr window = User32.GetWindow(_hostActiveWindowHandle, 3);
                if (User32.SetWindowPos(_dialogWindowHandle, window != IntPtr.Zero ? window : new IntPtr(1), 0, 0, 0, 0, 19))
                    return;
                User32.SetWindowPos(_dialogWindowHandle, new IntPtr(1), 0, 0, 0, 0, 19);
            }
        }

        private void PositionAndShowDialog()
        {
            Topmost = true;
            if (_hostActiveWindowHandle == IntPtr.Zero || !User32.GetWindowRect(_hostActiveWindowHandle, out var lpRect) || (lpRect.Width == 0 || lpRect.Height == 0))
                User32.GetWindowRect(User32.GetDesktopWindow(), out lpRect);
            var logicalUnits = new Rect(lpRect.Left, lpRect.Top, lpRect.Width, lpRect.Height).DeviceToLogicalUnits();
            var num = double.IsNaN(Height) ? MinHeight : Height;
            Top = logicalUnits.Top + (logicalUnits.Height - num) / 2.0;
            Left = logicalUnits.Left + (logicalUnits.Width - Width) / 2.0;
            Show();
        }

        private bool CanShowDialog()
        {
            if (_hostMainWindowHandle == IntPtr.Zero || !User32.IsWindowVisible(_hostMainWindowHandle))
                return true;
            if (!IsHostProcessForeground())
                return false;
            return GetMainThreadActiveWindow(_hostActiveWindowHandle) == _hostActiveWindowHandle;
        }

        private void CaptionArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private static IntPtr GetMainThreadActiveWindow(IntPtr activeWindowHandle)
        {
            var windowThreadProcessId = User32.GetWindowThreadProcessId(activeWindowHandle, out _);
            var lpgui = new GuiThreadInfo
            {
                CbSize = Marshal.SizeOf(typeof(GuiThreadInfo))
            };
            if (windowThreadProcessId != 0U && User32.GetGUIThreadInfo(windowThreadProcessId, out lpgui))
                return lpgui.HwndActive;
            return IntPtr.Zero;
        }

        private bool IsHostProcessForeground()
        {
            var foregroundWindow = User32.GetForegroundWindow();
            User32.GetWindowThreadProcessId(foregroundWindow, out var processId);
            if (processId == _hostProcessId || foregroundWindow == _dialogWindowHandle)
                return true;
            return IsGhostWindow(foregroundWindow);
        }

        private bool IsGhostWindow(IntPtr candidateHandle)
        {
            if (candidateHandle == IntPtr.Zero)
                return false;
            var lpString = new StringBuilder();
            if (User32.GetClassName(candidateHandle, lpString, 6) != 5 || lpString.ToString() != "Ghost")
                return false;
            candidateHandle = GetRootOwnerWindow(candidateHandle);
            var rootOwnerWindow = GetRootOwnerWindow(_hostActiveWindowHandle);
            User32.GetWindowRect(candidateHandle, out var lpRect1);
            User32.GetWindowRect(rootOwnerWindow, out var lpRect2);
            return lpRect1.Size == lpRect2.Size && User32.GetWindowText(candidateHandle).StartsWith(_hostRootWindowCaption);
        }

        private static IntPtr GetRootOwnerWindow(IntPtr handle)
        {
            while (true)
            {
                var window = User32.GetWindow(handle, 4);
                if (!(window == IntPtr.Zero))
                    handle = window;
                else
                    break;
            }
            return handle;
        }
    }
}

