using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using ModernApplicationFramework.Utilities.NativeMethods;

namespace ModernApplicationFramework.Utilities
{
    public static class FocusHelper
    {
        public static readonly DependencyProperty FocusTargetProperty = DependencyProperty.RegisterAttached("FocusTarget", typeof(UIElement), typeof(FocusHelper), new FrameworkPropertyMetadata(null, OnFocusTargetChanged));

        public static UIElement GetFocusTarget(UIElement element)
        {
            Validate.IsNotNull(element, "element");
            return (UIElement) element.GetValue(FocusTargetProperty);
        }

        public static void SetFocusTarget(UIElement element, UIElement value)
        {
            Validate.IsNotNull(element, "element");
            element.SetValue(FocusTargetProperty, value);
        }

        public static void FocusPossiblyUnloadedElement(FrameworkElement element)
        {
            Validate.IsNotNull(element, "element");
            PendingFocusHelper.SetFocusOnLoad(element);
        }

        public static void MoveFocusInto(UIElement element)
        {
            if (IsKeyboardFocusWithin(element))
                return;
            element.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        public static bool IsKeyboardFocusWithin(UIElement element)
        {
            if (element.IsKeyboardFocusWithin)
                return true;
            var focus = User32.GetFocus();
            return element.FindDescendants<HwndHost>().Any(descendant => IsChildOrSame(descendant.Handle, focus));
        }

        public static bool IsKeyboardFocusWithin(IntPtr hwnd)
        {
            return IsChildOrSame(hwnd, User32.GetFocus());
        }

        private static bool IsChildOrSame(IntPtr hwndParent, IntPtr hwndChild)
        {
            return hwndParent == hwndChild || User32.IsChild(hwndParent, hwndChild);
        }

        private static void OnFocusTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (UIElement)d;
            if (e.NewValue == null)
            {
                if (e.OldValue == null)
                    return;
                WeakEventManager<UIElement, KeyboardFocusChangedEventArgs>.RemoveHandler(source, "GotKeyboardFocus", OnGotKeyboardFocus);
            }
            else
            {
                if (e.OldValue != null)
                    return;
                WeakEventManager<UIElement, KeyboardFocusChangedEventArgs>.AddHandler(source, "GotKeyboardFocus", OnGotKeyboardFocus);
            }
        }

        private static void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var element = (UIElement)sender;
            if (!Equals(e.OriginalSource, element))
                return;
            MoveFocusInto(GetFocusTarget(element));
        }
    }

    public static class PendingFocusHelper
    {
        public static readonly DispatcherPriority FocusPriority = DispatcherPriority.Loaded;
        private static FrameworkElement _pendingFocusElement;

        private static Action<FrameworkElement> PendingFocusAction { get; set; }

        private static FrameworkElement PendingFocusElement
        {
            get => _pendingFocusElement;
            set
            {
                if (Equals(_pendingFocusElement, value))
                    return;
                if (_pendingFocusElement != null)
                    _pendingFocusElement.Loaded -= OnPendingFocusElementLoaded;
                _pendingFocusElement = value;
                if (_pendingFocusElement == null)
                    return;
                _pendingFocusElement.Loaded += OnPendingFocusElementLoaded;
            }
        }

        private static void OnPendingFocusElementLoaded(object sender, RoutedEventArgs args)
        {
            FrameworkElement pendingFocusElement = PendingFocusElement;
            if (pendingFocusElement != null)
                MoveFocusInto(pendingFocusElement, PendingFocusAction);
            PendingFocusElement = null;
            PendingFocusAction = null;
        }

        private static void MoveFocusInto(FrameworkElement element, Action<FrameworkElement> focusAction)
        {
            if (focusAction != null)
                focusAction(element);
            else
                FocusHelper.MoveFocusInto(element);
        }

        public static void SetFocusOnLoad(FrameworkElement element, Action<FrameworkElement> focusAction = null)
        {
            Validate.IsNotNull(element, "element");
            if (element.IsLoaded && element.IsConnectedToPresentationSource())
            {
                PendingFocusElement = null;
                PendingFocusAction = null;
                MoveFocusInto(element, focusAction);
            }
            else
            {
                PendingFocusElement = element;
                PendingFocusAction = focusAction;
            }
        }
    }
}
