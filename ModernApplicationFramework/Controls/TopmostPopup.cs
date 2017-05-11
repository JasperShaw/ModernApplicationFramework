using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Platform.Structs;

namespace ModernApplicationFramework.Controls
{
    public class TopmostPopup : Popup
    {
        public static readonly DependencyProperty TopmostProperty = DependencyProperty.Register("Topmost", typeof(bool),
            typeof(TopmostPopup), new FrameworkPropertyMetadata(false, OnTopmostChanged));

        private bool? _appliedTopMost;
        private bool _alreadyLoaded;
        private Window _parentWindow;

        public bool Topmost
        {
            get => (bool)GetValue(TopmostProperty);
            set => SetValue(TopmostProperty, value);
        }

        public TopmostPopup()
        {
            Loaded += OnPopupLoaded;
        }

        void OnPopupLoaded(object sender, RoutedEventArgs e)
        {
            if (_alreadyLoaded)
                return;

            _alreadyLoaded = true;

            Child?.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(OnChildPreviewMouseLeftButtonDown), true);

            _parentWindow = Window.GetWindow(this);

            if (_parentWindow == null)
                return;

            _parentWindow.Activated += OnParentWindowActivated;
            _parentWindow.Deactivated += OnParentWindowDeactivated;
        }

        void OnParentWindowActivated(object sender, EventArgs e)
        {
            SetTopmostState(true);
        }

        void OnParentWindowDeactivated(object sender, EventArgs e)
        {
            if (Topmost == false)
            {
                SetTopmostState(Topmost);
            }
        }

        void OnChildPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetTopmostState(true);

            if (!_parentWindow.IsActive && Topmost == false)
            {
                _parentWindow.Activate();
            }
        }

        private static void OnTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var thisobj = (TopmostPopup)obj;

            thisobj.SetTopmostState(thisobj.Topmost);
        }

        protected override void OnOpened(EventArgs e)
        {
            SetTopmostState(Topmost);
        }

        private void SetTopmostState(bool isTop)
        {
            // Don’t apply state if it’s the same as incoming state
            if (_appliedTopMost.HasValue && _appliedTopMost == isTop)
            {
                return;
            }

            if (Child == null)
                return;

            var hwndSource = (PresentationSource.FromVisual(Child)) as HwndSource;

            if (hwndSource == null)
                return;
            var hwnd = hwndSource.Handle;

            RECT rect;

            if (!User32.GetWindowRect(hwnd, out rect))
                return;
            if (isTop)
                User32.SetWindowPos(hwnd, new IntPtr(-1), rect.Left, rect.Top, (int)Width, (int)Height, 1563);
            else
            {
                User32.SetWindowPos(hwnd, new IntPtr(1), rect.Left, rect.Top, (int)Width, (int)Height, 1563);
                User32.SetWindowPos(hwnd, new IntPtr(0), rect.Left, rect.Top, (int)Width, (int)Height, 1563);
                User32.SetWindowPos(hwnd, new IntPtr(-2), rect.Left, rect.Top, (int)Width, (int)Height, 1563);
            }
            _appliedTopMost = isTop;
        }
    }
}
