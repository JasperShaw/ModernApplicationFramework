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
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Docking.Layout;
using NativeMethods = ModernApplicationFramework.Core.Standard.NativeMethods;
using SystemCommands = ModernApplicationFramework.Core.Shell.SystemCommands;

namespace ModernApplicationFramework.Docking.Controls
{
    public abstract class LayoutFloatingWindowControl : ModernChromeWindow, ILayoutControl
    {
        private readonly ILayoutElement _model;

        private static readonly DependencyPropertyKey IsDraggingPropertyKey
            = DependencyProperty.RegisterReadOnly("IsDragging", typeof (bool), typeof (LayoutFloatingWindowControl),
                new FrameworkPropertyMetadata(false,
                    OnIsDraggingChanged));

        public static readonly DependencyProperty IsDraggingProperty
            = IsDraggingPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey IsMaximizedPropertyKey
            = DependencyProperty.RegisterReadOnly("IsMaximized", typeof (bool), typeof (LayoutFloatingWindowControl),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsMaximizedProperty
            = IsMaximizedPropertyKey.DependencyProperty;


        private bool _attachDrag;
        private DragService _dragService;
        private HwndSource _hwndSrc;
        private HwndSourceHook _hwndSrcHook;
        private bool _internalCloseFlag;

        protected LayoutFloatingWindowControl(ILayoutElement model)
        {
            _model = model;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        static LayoutFloatingWindowControl()
        {
            ContentProperty.OverrideMetadata(typeof (LayoutFloatingWindowControl),
                new FrameworkPropertyMetadata(null, null, CoerceContentValue));
            AllowsTransparencyProperty.OverrideMetadata(typeof (LayoutFloatingWindowControl),
                new FrameworkPropertyMetadata(false));
            ShowInTaskbarProperty.OverrideMetadata(typeof (LayoutFloatingWindowControl),
                new FrameworkPropertyMetadata(false));
        }

        public abstract ILayoutElement Model { get; }
        public bool IsDragging => (bool) GetValue(IsDraggingProperty);
        public bool IsMaximized => (bool) GetValue(IsMaximizedProperty);
        protected bool CloseInitiatedByUser => !_internalCloseFlag;
        internal bool KeepContentVisibleOnClose { get; set; }

        public override void ChangeTheme(Theme oldValue, Theme newValue)
        {
            if (oldValue != null)
            {
                var resourceDictionaryToRemove =
                    Resources.MergedDictionaries.FirstOrDefault(r => r.Source == oldValue.GetResourceUri());
                if (resourceDictionaryToRemove != null)
                    Resources.MergedDictionaries.Remove(
                        resourceDictionaryToRemove);
            }
            if (newValue == null)
                return;
            Resources.MergedDictionaries.Add(new ResourceDictionary {Source = newValue.GetResourceUri()});
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            var hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            var keyboardInputSink = (IKeyboardInputSink)hwndSource;
            keyboardInputSink?.RegisterKeyboardInputSink(new MnemonicForwardingKeyboardInputSink(this));
            UpdateClipRegion();
            base.OnSourceInitialized(e);
        }

        protected virtual IntPtr FilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;

            switch (msg)
            {
                //VS Does not behave like this, so commented out
                case Win32Helper.WmActivate:
                    if (((int) wParam & 0xFFFF) == Win32Helper.WaInactive)
                    {
                        //if (lParam == this.GetParentWindowHandle())
                        //{
                        //	Win32Helper.SetActiveWindow(_hwndSrc.Handle);
                        //	handled = true;
                        //}
                    }
                    break;
                case Win32Helper.WmExitsizemove:
                    UpdatePositionAndSizeOfPanes();

                    if (_dragService != null)
                    {
                        bool dropFlag;
                        var mousePosition = this.TransformToDeviceDpi(Win32Helper.GetMousePosition());
                        _dragService.Drop(mousePosition, out dropFlag);
                        _dragService = null;
                        SetIsDragging(false);

                        if (dropFlag)
                            InternalClose();
                    }

                    break;
                case Win32Helper.WmMoving:
                {
                    UpdateDragPosition();
                }
                    break;
                case Win32Helper.WmLbuttonup:
                    //set as handled right button click on title area (after showing context menu)
                    if (_dragService != null && Mouse.LeftButton == MouseButtonState.Released)
                    {
                        _dragService.Abort();
                        _dragService = null;
                        SetIsDragging(false);
                    }
                    break;
                case Win32Helper.WmSyscommand:
                    IntPtr wMaximize = new IntPtr(Win32Helper.ScMaximize);
                    IntPtr wRestore = new IntPtr(Win32Helper.ScRestore);
                    if (wParam == wMaximize || wParam == wRestore)
                    {
                        UpdateMaximizedState(wParam == wMaximize);
                    }
                    break;
                case Win32Helper.WmNclbuttondblclk:
                    if (msg != 24)
                    {
                        if (msg == Win32Helper.WmNclbuttondblclk)
                        {
                            WmNcLButtonDblClk(wParam, ref handled);
                            return IntPtr.Zero;
                        }
                    }
                    else
                    {
                        WmShowWindow(hwnd, lParam);
                        return IntPtr.Zero;
                    }
                    break;
            }


            return IntPtr.Zero;
        }

        private void WmShowWindow(IntPtr hwnd, IntPtr lParam)
        {
            if (lParam.ToInt32() == 3)
            {
                if (WindowState == WindowState.Maximized)
                {
                    ShowActivated = true;
                    NativeMethods.ShowWindow(hwnd, 3);
                }
                else
                    NativeMethods.ShowWindow(hwnd, 9);
            }
            else
            {
                if (lParam.ToInt32() != 1)
                    return;
                NativeMethods.ShowWindow(hwnd, 6);
            }
        }

        private void WmNcLButtonDblClk(IntPtr wParam, ref bool handled)
        {
            if (!NativeMethods.IsKeyPressed(17) || wParam.ToInt32() != 2)
                return;
            RedockWindow();
            handled = true;

        }

        protected abstract void RedockWindow();

        protected override void OnClosed(EventArgs e)
        {
            if (Content != null)
            {
                var host = Content as FloatingWindowContentHost;
                host?.Dispose();

                if (_hwndSrc != null)
                {
                    _hwndSrc.RemoveHook(_hwndSrcHook);
                    _hwndSrc.Dispose();
                    _hwndSrc = null;
                }
            }

            base.OnClosed(e);
        }

        protected override void OnInitialized(EventArgs e)
        {
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand,
                (s, args) => SystemCommands.CloseWindow((Window) args.Parameter)));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand,
                (s, args) => SystemCommands.MaximizeWindow((Window) args.Parameter)));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand,
                (s, args) => SystemCommands.MinimizeWindow((Window) args.Parameter)));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand,
                (s, args) => SystemCommands.RestoreWindow((Window) args.Parameter)));
            //Debug.Assert(this.Owner != null);
            base.OnInitialized(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            // Sice they should not be used as a MainWindow we can do this in order to provide the Inputbindings from the MainWindow working
            Keyboard.Focus(Application.Current.MainWindow);
        }

        protected virtual void OnIsDraggingChanged(DependencyPropertyChangedEventArgs e)
        {
            //Trace.WriteLine("IsDragging={0}", e.NewValue);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            SetIsMaximized(WindowState == WindowState.Maximized);
            base.OnStateChanged(e);
        }

        protected void SetIsDragging(bool value)
        {
            SetValue(IsDraggingPropertyKey, value);
        }

        protected void SetIsMaximized(bool value)
        {
            SetValue(IsMaximizedPropertyKey, value);
        }

        protected override bool UpdateClipRegionCore(IntPtr hWnd, int showCmd, ClipRegionChangeType changeType,
            Int32Rect currentBounds)
        {
            return false;
        }

        internal void AttachDrag(bool onActivated = true)
        {
            if (onActivated)
            {
                _attachDrag = true;
                Activated += OnActivated;
            }
            else
            {
                IntPtr windowHandle = new WindowInteropHelper(this).EnsureHandle();
                IntPtr lParam = new IntPtr(((int) Left & 0xFFFF) | (((int) Top) << 16));
                Win32Helper.SendMessage(windowHandle, Win32Helper.WmNclbuttondown, new IntPtr(Win32Helper.HtCaption),
                    lParam);
            }
        }

        internal void InternalClose()
        {
            _internalCloseFlag = true;
            Close();
        }

        private static object CoerceContentValue(DependencyObject sender, object content)
        {
            return new FloatingWindowContentHost(sender as LayoutFloatingWindowControl) {Content = content as UIElement};
        }

        private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutFloatingWindowControl) d).OnIsDraggingChanged(e);
        }

        private void OnActivated(object sender, EventArgs e)
        {
            Activated -= OnActivated;

            if (!_attachDrag || Mouse.LeftButton != MouseButtonState.Pressed)
                return;
            IntPtr windowHandle = new WindowInteropHelper(this).EnsureHandle();
            var mousePosition = this.PointToScreenDpi(Mouse.GetPosition(this));
            var clientArea = Win32Helper.GetClientRect(windowHandle);
            var windowArea = Win32Helper.GetWindowRect(windowHandle);

            Left = mousePosition.X - windowArea.Width/2.0;
            Top = mousePosition.Y - (windowArea.Height - clientArea.Height)/2.0;
            _attachDrag = false;

            IntPtr lParam = new IntPtr(((int) mousePosition.X & 0xFFFF) | (((int) mousePosition.Y) << 16));
            Win32Helper.SendMessage(windowHandle, Win32Helper.WmNclbuttondown, new IntPtr(Win32Helper.HtCaption), lParam);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            ChangeTheme(null, null);
            this.SetParentToMainWindowOf(Model.Root.Manager);
            _hwndSrc = PresentationSource.FromDependencyObject(this) as HwndSource;
            _hwndSrcHook = FilterMessage;
            _hwndSrc?.AddHook(_hwndSrcHook);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= OnUnloaded;

            if (_hwndSrc == null)
                return;
            _hwndSrc.RemoveHook(_hwndSrcHook);
            _hwndSrc.Dispose();
            _hwndSrc = null;
        }

        private void UpdateDragPosition()
        {
            if (_dragService == null)
            {
                _dragService = new DragService(this);
                SetIsDragging(true);
            }

            var mousePosition = this.TransformToDeviceDpi(Win32Helper.GetMousePosition());
            _dragService.UpdateMouseLocation(mousePosition);
        }

        private void UpdateMaximizedState(bool isMaximized)
        {
            foreach (var posElement in Model.Descendents().OfType<ILayoutElementForFloatingWindow>())
            {
                posElement.IsMaximized = isMaximized;
            }
        }

        private void UpdatePositionAndSizeOfPanes()
        {
            foreach (var posElement in Model.Descendents().OfType<ILayoutElementForFloatingWindow>())
            {
                posElement.FloatingLeft = Left;
                posElement.FloatingTop = Top;
                posElement.FloatingWidth = Width;
                posElement.FloatingHeight = Height;
            }
        }

        protected class FloatingWindowContentHost : HwndHost
        {
            public static readonly DependencyProperty ContentProperty =
                DependencyProperty.Register("Content", typeof (UIElement), typeof (FloatingWindowContentHost),
                    new FrameworkPropertyMetadata(null,
                        OnContentChanged));

            private readonly LayoutFloatingWindowControl _owner;
            private DockingManager _manager;
            private Border _rootPresenter;
            private HwndSource _wpfContentHost;

            public FloatingWindowContentHost(LayoutFloatingWindowControl owner)
            {
                _owner = owner;
            }

            public Visual RootVisual => _rootPresenter;

            public UIElement Content
            {
                get => (UIElement) GetValue(ContentProperty);
                set => SetValue(ContentProperty, value);
            }

            protected override HandleRef BuildWindowCore(HandleRef hwndParent)
            {
                _wpfContentHost = new HwndSource(new HwndSourceParameters
                {
                    ParentWindow = hwndParent.Handle,
                    WindowStyle =
                        Win32Helper.WsChild | Win32Helper.WsVisible | Win32Helper.WsClipsiblings |
                        Win32Helper.WsClipchildren,
                    Width = 1,
                    Height = 1
                });

                _rootPresenter = new Border {Child = new AdornerDecorator {Child = Content}, Focusable = true};
                _rootPresenter.SetBinding(Border.BackgroundProperty, new Binding("Background") {Source = _owner});
                _wpfContentHost.RootVisual = _rootPresenter;
                _wpfContentHost.SizeToContent = SizeToContent.Manual;
                _manager = _owner.Model.Root.Manager;
                _manager.InternalAddLogicalChild(_rootPresenter);

                return new HandleRef(this, _wpfContentHost.Handle);
            }

            protected override void DestroyWindowCore(HandleRef hwnd)
            {
                _manager.InternalRemoveLogicalChild(_rootPresenter);
                if (_wpfContentHost != null)
                {
                    _wpfContentHost.Dispose();
                    _wpfContentHost = null;
                }
            }

            protected override Size MeasureOverride(Size constraint)
            {
                if (Content == null)
                    return base.MeasureOverride(constraint);

                Content.Measure(constraint);
                return Content.DesiredSize;
            }

            protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e)
            {
                if (_rootPresenter != null)
                    _rootPresenter.Child = Content;
            }

            protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
            {
                Trace.WriteLine("FloatingWindowContentHost.GotKeyboardFocus");
                base.OnGotKeyboardFocus(e);
            }

            protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
            {
                switch (msg)
                {
                    case Win32Helper.WmSetfocus:
                        Trace.WriteLine("FloatingWindowContentHost.WM_SETFOCUS");
                        break;
                    case Win32Helper.WmKillfocus:
                        Trace.WriteLine("FloatingWindowContentHost.WM_KILLFOCUS");
                        break;
                }
                return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
            }

            private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                ((FloatingWindowContentHost) d).OnContentChanged(e);
            }
        }

        private class MnemonicForwardingKeyboardInputSink : UIElement, IKeyboardInputSink
        {

            private Window Window { get; }

            IKeyboardInputSite IKeyboardInputSink.KeyboardInputSite { get; set; }

            public MnemonicForwardingKeyboardInputSink(Window window)
            {
                Window = window;
            }

            bool IKeyboardInputSink.HasFocusWithin()
            {
                return false;
            }

            public IKeyboardInputSite RegisterKeyboardInputSink(IKeyboardInputSink sink)
            {
                throw new NotSupportedException();
            }

            public bool TranslateAccelerator(ref MSG msg, ModifierKeys modifiers)
            {
                return false;
            }

            public bool TabInto(TraversalRequest request)
            {
                return false;
            }

            public bool OnMnemonic(ref MSG msg, ModifierKeys modifiers)
            {
                switch (msg.message)
                {
                    case 262:
                    case 263:
                        string key = new string((char)(int)msg.wParam, 1);
                        if (key.Length > 0)
                        {
                            IntPtr hwnd = new WindowInteropHelper(Window).Owner;
                            if (hwnd == IntPtr.Zero)
                            {
                                var fromVisual = (HwndSource)PresentationSource.FromVisual(Application.Current.MainWindow);
                                if (fromVisual != null)
                                    hwnd =  fromVisual.Handle;
                            }
                            if (hwnd != IntPtr.Zero)
                            {
                                HwndSource hwndSource = HwndSource.FromHwnd(hwnd);
                                if (hwndSource != null && AccessKeyManager.IsKeyRegistered(hwndSource, key))
                                {
                                    AccessKeyManager.ProcessKey(hwndSource, key, false);
                                    return true;
                                }
                            }
                        }
                        break;
                }
                return false;
            }

            public bool TranslateChar(ref MSG msg, ModifierKeys modifiers)
            {
                return false;
            }
        }
    }
}