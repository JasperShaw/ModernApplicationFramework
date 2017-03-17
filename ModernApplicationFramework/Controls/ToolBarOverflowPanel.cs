using System.Windows;
using System.Windows.Input;

namespace ModernApplicationFramework.Controls
{
    public class ToolBarOverflowPanel : System.Windows.Controls.Primitives.ToolBarOverflowPanel
    {
        private bool _wasShiftDown;
        private Key? _lastSeenKey;

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            UIElement uiElement = oldParent as UIElement;
            if (uiElement != null)
            {
                uiElement.PreviewKeyDown -= OnPreviewKeyDownInParentScope;
                uiElement.IsKeyboardFocusWithinChanged -= OnParentKeyboardFocusWithinChanged;
            }
            UIElement parent = Parent as UIElement;
            if (parent != null)
            {
                parent.PreviewKeyDown += OnPreviewKeyDownInParentScope;
                parent.IsKeyboardFocusWithinChanged += OnParentKeyboardFocusWithinChanged;
            }
            base.OnVisualParentChanged(oldParent);
        }


        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (!Equals(e.NewFocus, this) || !_lastSeenKey.HasValue)
                return;
            PerformFocusForwarding(_lastSeenKey.Value, _wasShiftDown);
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (IsFocused)
                e.Handled = PerformFocusForwarding(e.Key, Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
            if (e.Handled)
                return;
            base.OnKeyDown(e);
        }

        private void OnParentKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                return;
            _lastSeenKey = new Key?();
            _wasShiftDown = false;
        }

        private void OnPreviewKeyDownInParentScope(object sender, KeyEventArgs e)
        {
            _lastSeenKey = e.Key;
            _wasShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        }

        private bool PerformFocusForwarding(Key trigger, bool isShiftDown)
        {
            bool flag = true;
            FocusNavigationDirection focusNavigationDirection = FocusNavigationDirection.Next;
            switch (trigger)
            {
                case Key.Tab:
                    if (isShiftDown)
                    {
                        focusNavigationDirection = FocusNavigationDirection.Previous;
                        goto case Key.Right;
                    }
                    else
                        goto case Key.Right;
                case Key.Left:
                case Key.Up:
                    focusNavigationDirection = FocusNavigationDirection.Previous;
                    goto case Key.Right;
                case Key.Right:
                case Key.Down:
                    if (flag)
                        return MoveFocus(new TraversalRequest(focusNavigationDirection));
                    return false;
                default:
                    flag = false;
                    goto case Key.Right;
            }
        }

    }
}
