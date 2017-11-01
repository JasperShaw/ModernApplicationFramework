using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Platform.Enums;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Core.Utilities
{
    internal class HwndSourceTracker : DisposableObject
    {
        private static readonly int PrivateRestoreFocusMessage = User32.RegisterWindowMessage("VSM_TRYRESTOREFOCUS");
        private static readonly Dictionary<HwndSource, HwndSourceTracker> TrackedSources = new Dictionary<HwndSource, HwndSourceTracker>();
        private bool _cancelRestoreFocus;
        private bool _isPendingRestoreFocus;

        private HwndSource Source { get; }

        private RestoreFocusScope RestoreFocusScope { get; set; }

        private RestoreFocusScope RestoreActivationScope { get; set; }


        public static void Initialize()
        {
            FocusTracker.Instance.TrackFocus += OnFocusChanged;
        }

        private static void OnFocusChanged(object sender, TrackFocusEventArgs args)
        {
            TrackSource(args.HwndGainFocus == IntPtr.Zero ? null : HwndSource.FromHwnd(args.HwndGainFocus));
            PossiblyUpdateRestoreFocus(args.HwndGainFocus);
        }

        private static void TrackSource(HwndSource source)
        {
            if (source == null || source.RestoreFocusMode != RestoreFocusMode.None || TrackedSources.ContainsKey(source))
                return;
            TrackedSources.Add(source, new HwndSourceTracker(source));
            source.Disposed += OnSourceDisposed;
        }

        public HwndSourceTracker(HwndSource source)
        {
            Source = source;
            source.AddHook(HwndSource_MessageHook);
        }

        private IntPtr HwndSource_MessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 516:
                case 519:
                case 523:
                case 171:
                case 513:
                case 161:
                case 164:
                case 167:
                    HandleButtonDown();
                    break;
                case 6:
                    handled = HandleActivate(wParam, lParam);
                    break;
                case 7:
                    HandleSetFocus(wParam);
                    break;
                case 8:
                    HandleKillFocus(wParam);
                    break;
                case 33:
                    return HandleMouseActivate(ref handled);
                default:
                    if (msg == PrivateRestoreFocusMessage)
                    {
                        HandleRestoreFocus();
                        handled = true;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void HandleSetFocus(IntPtr wParam)
        {
            if (_isPendingRestoreFocus)
                return;
            if (!IsValidRestoreTarget(Source, wParam))
            {
                _isPendingRestoreFocus = true;
                User32.PostMessage(Source.Handle, PrivateRestoreFocusMessage, IntPtr.Zero, IntPtr.Zero);
            }
            else
                RestoreFocusScope = null;
        }

        private void HandleKillFocus(IntPtr wParam)
        {
            if (!IsValidRestoreTarget(Source, Keyboard.FocusedElement) || IsValidRestoreTarget(Source, wParam))
                return;
            RestoreFocusScope = new HwndSourceRestoreFocusScope(Source, Keyboard.FocusedElement, IntPtr.Zero);
        }

        private void HandleRestoreFocus()
        {
            if (InputManager.Current.IsInMenuMode)
            {
                void LeaveMenuMode(object param1, EventArgs param2)
                {
                    InputManager.Current.LeaveMenuMode -= LeaveMenuMode;
                    HandleRestoreFocusCore();
                }

                InputManager.Current.LeaveMenuMode += LeaveMenuMode;
            }
            else
                HandleRestoreFocusCore();
        }

        private void HandleRestoreFocusCore()
        {
            _isPendingRestoreFocus = false;
            if (ShouldRestoreFocus())
            {
                if (RestoreFocusScope != null)
                    RestoreFocusScope.PerformRestoration();
                else
                    Keyboard.Focus(null);
            }
            if (ShouldSynchronizeActivation())
            {
               // WindowManagerService.Instance.ActivateFrameFromSource(this.Source);
            }         
            _cancelRestoreFocus = false;
        }

        private bool ShouldSynchronizeActivation()
        {
            if (_cancelRestoreFocus || !(User32.GetActiveWindow() == Source.Handle))
                return false;
            return true;
        }

        private bool ShouldRestoreFocus()
        {
            if (!_cancelRestoreFocus && User32.GetFocus() == Source.Handle)
                return !IsKeyboardFocusInSource(Source);
            return false;
        }

        private static bool IsKeyboardFocusInSource(HwndSource source)
        {
            if (!(Keyboard.FocusedElement is DependencyObject focusedElement))
                return false;
            HwndSource hwndSource = (HwndSource)PresentationSource.FromDependencyObject(focusedElement);
            if (hwndSource == null)
                return false;
            if (hwndSource != source)
                return User32.IsChild(source.Handle, hwndSource.Handle);
            return true;
        }

        private static bool IsLosingAggregateActivation(IntPtr losingHwnd, IntPtr gainingHwnd)
        {
            return IsGainingAggregateActivation(gainingHwnd, losingHwnd);
        }

        private static bool IsGainingAggregateActivation(IntPtr losingHwnd, IntPtr gainingHwnd)
        {
            if (losingHwnd == IntPtr.Zero)
                return true;
            return GetRootOwner(losingHwnd) != GetRootOwner(gainingHwnd);
        }

        internal static IntPtr GetRootOwner(IntPtr hwnd)
        {
            hwnd = User32.GetAncestor(hwnd, 2);
            IntPtr num = hwnd;
            for (; hwnd != IntPtr.Zero; hwnd = User32.GetWindow(hwnd, 4))
                num = hwnd;
            return num;
        }

        private HwndSourceTracker GetRootTracker()
        {
            var key = HwndSource.FromHwnd(GetRootOwner(Source.Handle));
            if (key != null && TrackedSources.TryGetValue(key, out var hwndSourceTracker))
                return hwndSourceTracker;
            return null;
        }

        private bool HandleActivate(IntPtr wParam, IntPtr lParam)
        {
            switch (wParam.ToInt32())
            {
                case 0:
                    if (IsLosingAggregateActivation(Source.Handle, lParam))
                    {
                        HwndSourceTracker rootTracker = GetRootTracker();
                        if (rootTracker != null)
                        {
                            rootTracker.RestoreActivationScope = rootTracker.Source != Source ? new HwndSourceRestoreActivationScope(Source.Handle) : null;
                        }
                    }
                    break;
                case 1:
                    if (IsGainingAggregateActivation(lParam, Source.Handle) && RestoreActivationScope != null)
                    {
                        RestoreFocusScope restoreActivationScope = RestoreActivationScope;
                        RestoreActivationScope = null;
                        restoreActivationScope.PerformRestoration();
                        return true;
                    }
                    break;
                case 2:
                    RestoreActivationScope = null;
                    break;
            }
            return false;
        }

        private IntPtr HandleMouseActivate(ref bool handled)
        {
            if (User32.GetFocus() != IntPtr.Zero)
            {
                if (CommandFocusManager.IsInsideCommandContainer(Mouse.DirectlyOver))
                {
                    handled = true;
                    return new IntPtr(3);
                }
                if (CommandFocusManager.IsInsideCommandContainer(Keyboard.FocusedElement))
                    Keyboard.Focus(null);
            }
            return IntPtr.Zero;
        }

        private void HandleButtonDown()
        {
            if (!(User32.GetFocus() != IntPtr.Zero) || CommandFocusManager.IsInsideCommandContainer(Mouse.DirectlyOver) || !CommandFocusManager.IsInsideCommandContainer(Keyboard.FocusedElement))
                return;
            Keyboard.Focus(null);
        }

        private static void OnSourceDisposed(object sender, EventArgs args)
        {
            HwndSource key = (HwndSource)sender;
            key.Disposed -= OnSourceDisposed;
            TrackedSources[key].Dispose();
            TrackedSources.Remove(key);
        }

        private static void PossiblyUpdateRestoreFocus(IntPtr hwndGainFocus)
        {
            foreach (KeyValuePair<HwndSource, HwndSourceTracker> trackedSource in TrackedSources)
            {
                if (IsValidRestoreTarget(trackedSource.Key, hwndGainFocus))
                    trackedSource.Value.RestoreFocusScope = new HwndSourceRestoreFocusScope(trackedSource.Key, null, GetImmediateChildFor(hwndGainFocus, trackedSource.Key.Handle));
            }
        }

        private static IntPtr GetImmediateChildFor(IntPtr hwnd, IntPtr hwndRoot)
        {
            IntPtr parent;
            for (; hwnd != IntPtr.Zero && (User32.GetWindowLong(hwnd, (int)Gwl.Style) & 1073741824) != 0; hwnd = parent)
            {
                parent = User32.GetParent(hwnd);
                if (parent == hwndRoot)
                    return hwnd;
            }
            return IntPtr.Zero;
        }

        internal static bool IsValidRestoreTarget(HwndSource source, IntPtr hwnd)
        {
            return User32.IsChild(source.Handle, hwnd);
        }

        private static bool IsValidRestoreTarget(HwndSource source, IInputElement element)
        {
            if (element is DependencyObject dependencyObject)
                return PresentationSource.FromDependencyObject(dependencyObject) == source;
            return false;
        }
    }


    public sealed class FocusTracker : DisposableObject
    {
        [ThreadStatic]
        private static FocusTracker _instance;
        private WindowsHookHandle _windowsHookHandle;
        private readonly NativeMethods.WindowsHookProc _cbtHookProc;

        public static FocusTracker Instance => _instance ?? (_instance = new FocusTracker());

        private FocusTracker()
        {
            _cbtHookProc = CbtWindowsHookProc;
        }

        public void SendFocusChangeNotification(IntPtr hwndGainFocus, IntPtr hwndLoseFocus)
        {
            SendTrackFocusEvent(new TrackFocusEventArgs(hwndGainFocus, hwndLoseFocus));
        }

        private IntPtr CbtWindowsHookProc(NativeMethods.CbtHookAction code, IntPtr wParam, IntPtr lParam)
        {
            if (code == NativeMethods.CbtHookAction.HCBT_SETFOCUS)
                SendFocusChangeNotification(wParam, lParam);
            return User32.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        private void SendTrackFocusEvent(TrackFocusEventArgs e)
        {
            EventHandler<TrackFocusEventArgs> trackFocusDelegate = TrackFocusDelegate;
            trackFocusDelegate?.Invoke(this, e);
        }

        private event EventHandler<TrackFocusEventArgs> TrackFocusDelegate;

        public event EventHandler<TrackFocusEventArgs> TrackFocus
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                TrackFocusDelegate += value;
                if (_windowsHookHandle != null && !_windowsHookHandle.IsInvalid)
                    return;
                _windowsHookHandle = new WindowsHookHandle(NativeMethods.WindowsHookType.WH_CBT, _cbtHookProc, IntPtr.Zero, Kernel32.GetCurrentThreadId());
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                TrackFocusDelegate -= value;
                if (TrackFocusDelegate != null || _windowsHookHandle == null)
                    return;
                _windowsHookHandle.Close();
                _windowsHookHandle = null;
            }
        }

        protected override void DisposeNativeResources()
        {
            if (_windowsHookHandle == null)
                return;
            _windowsHookHandle.Close();
            _windowsHookHandle = null;
        }

        private sealed class WindowsHookHandle : CriticalHandle
        {
            public WindowsHookHandle(NativeMethods.WindowsHookType hookType, NativeMethods.WindowsHookProc hookProc, IntPtr module, uint threadId)
                : base(IntPtr.Zero)
            {
                SetHandle(User32.SetWindowsHookEx(hookType, hookProc, module, threadId));
            }

            public override bool IsInvalid => handle == IntPtr.Zero;

            protected override bool ReleaseHandle()
            {
                return User32.UnhookWindowsHookEx(handle);
            }
        }
    }

    public class TrackFocusEventArgs : EventArgs
    {
        public TrackFocusEventArgs(IntPtr hwndGainFocus, IntPtr hwndLoseFocus)
        {
            HwndGainFocus = hwndGainFocus;
            HwndLoseFocus = hwndLoseFocus;
        }

        public IntPtr HwndGainFocus { get; set; }

        public IntPtr HwndLoseFocus { get; set; }
    }

    internal class HwndSourceRestoreFocusScope : RestoreFocusScope
    {
        public HwndSourceRestoreFocusScope(HwndSource source, IInputElement restoreFocus, IntPtr restoreFocusWindow)
            : base(restoreFocus, restoreFocusWindow)
        {
            Source = source;
        }

        private HwndSource Source { get; }

        protected override bool IsRestorationTargetValid()
        {
            if (RestoreFocusWindow != IntPtr.Zero)
                return HwndSourceTracker.IsValidRestoreTarget(Source, RestoreFocusWindow);
            if (RestoreFocus is DependencyObject restoreFocus)
                return PresentationSource.FromDependencyObject(restoreFocus) == Source;
            return false;
        }
    }

    internal class HwndSourceRestoreActivationScope : RestoreFocusScope
    {
        public HwndSourceRestoreActivationScope(IntPtr hwnd)
            : base(null, hwnd)
        {
        }

        protected override bool IsRestorationTargetValid()
        {
            return User32.IsWindow(RestoreFocusWindow);
        }
    }
}
