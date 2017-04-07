using System;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Native.Standard;

namespace ModernApplicationFramework.Core.Utilities
{
    public static class DteFocusHelper
    {
        public static readonly DependencyProperty AcquireFocusProperty;
        public static readonly RoutedEvent AcquireFocusEvent;

        static DteFocusHelper()
        {
            AcquireFocusProperty = DependencyProperty.RegisterAttached("AcquireFocus", typeof(bool), typeof(DteFocusHelper), new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnAcquireFocusPropertyChanged));
            AcquireFocusEvent = EventManager.RegisterRoutedEvent("AcquireFocus", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(DteFocusHelper));
        }

        private static void OnAcquireFocusPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            UIElement uiElement = obj as UIElement;
            if (uiElement == null)
                throw new ArgumentException("AcquireFocus element must be a UIElement.");
            if (!(bool)args.NewValue)
                return;
            uiElement.RaiseEvent(new RoutedEventArgs(AcquireFocusEvent));
        }

        public static bool GetAcquireFocus(UIElement obj)
        {
            return (bool)obj.GetValue(AcquireFocusProperty);
        }

        public static void SetAcquireFocus(UIElement obj, bool value)
        {
            obj.SetValue(AcquireFocusProperty, Boxes.Box(value));
        }

        public static void HookAcquireFocus(UIElement element)
        {
            element.AddHandler(AcquireFocusEvent, new RoutedEventHandler(SetFocus));
        }

        private static void SetFocus(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus((IInputElement)sender);
        }
    }
}
