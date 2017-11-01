using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Core.Utilities
{
    public static class CommandNavigationHelper
    {
        public static readonly DependencyProperty CommandFocusModeProperty = DependencyProperty.RegisterAttached("CommandFocusMode", typeof(CommandFocusMode), typeof(CommandNavigationHelper), new FrameworkPropertyMetadata(CommandFocusMode.None, OnCommandFocusModePropertyChanged));
        public static readonly DependencyProperty IsCommandNavigableProperty = DependencyProperty.RegisterAttached("IsCommandNavigable", typeof(bool), typeof(CommandNavigationHelper), new FrameworkPropertyMetadata(false, OnIsCommandNavigablePropertyChanged));
        public static readonly DependencyProperty CommandNavigationOrderProperty = DependencyProperty.RegisterAttached("CommandNavigationOrder", typeof(int), typeof(CommandNavigationHelper), new FrameworkPropertyMetadata(Boxes.Int32Zero, OnCommandNavigationOrderPropertyChanged));
        private static readonly WeakCollection<UIElement> NavigableControls = new WeakCollection<UIElement>();

        public static CommandFocusMode GetCommandFocusMode(DependencyObject element)
        {
            Validate.IsNotNull(element, nameof(element));
            return (CommandFocusMode)element.GetValue(CommandFocusModeProperty);
        }

        public static void SetCommandFocusMode(DependencyObject element, CommandFocusMode value)
        {
            Validate.IsNotNull(element, nameof(element));
            element.SetValue(CommandFocusModeProperty, value);
        }

        public static event PropertyChangedCallback CommandFocusModePropertyChanged;

        private static void OnCommandFocusModePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var modePropertyChanged = CommandFocusModePropertyChanged;
            modePropertyChanged?.Invoke(obj, e);
        }

        public static bool GetIsCommandNavigable(DependencyObject element)
        {
            Validate.IsNotNull(element, nameof(element));
            return (bool)element.GetValue(IsCommandNavigableProperty);
        }

        public static void SetIsCommandNavigable(DependencyObject element, bool value)
        {
            Validate.IsNotNull(element, nameof(element));
            element.SetValue(IsCommandNavigableProperty, value);
        }

        private static void OnIsCommandNavigablePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Validate.IsNotNull(obj, nameof(obj));
            if (!(obj is UIElement control))
                return;
            if (e.NewValue is bool b && b)
                RegisterNavigableControl(control);
            else
                UnregisterNavigableControl(control);
        }

        public static int GetCommandNavigationOrder(DependencyObject element)
        {
            Validate.IsNotNull(element, nameof(element));
            return (int)element.GetValue(CommandNavigationOrderProperty);
        }

        public static void SetCommandNavigationOrder(DependencyObject element, int value)
        {
            Validate.IsNotNull(element, nameof(element));
            element.SetValue(CommandNavigationOrderProperty, value);
            IsCommandNavigationOrderDirty = true;
        }

        private static void OnCommandNavigationOrderPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            IsCommandNavigationOrderDirty = true;
        }

        private static bool IsCommandNavigationOrderDirty { get; set; }

        private static void RegisterNavigableControl(UIElement control)
        {
            NavigableControls.Add(control);
            IsCommandNavigationOrderDirty = true;
        }

        private static void UnregisterNavigableControl(UIElement control)
        {
            NavigableControls.Remove(control);
            IsCommandNavigationOrderDirty = true;
        }

        internal static IEnumerable<UIElement> GetSortedNavigableControls()
        {
            if (IsCommandNavigationOrderDirty)
            {
                List<UIElement> list = NavigableControls.OrderBy(GetCommandNavigationOrder).ToList();
                NavigableControls.Clear();
                foreach (UIElement uiElement in list)
                    NavigableControls.Add(uiElement);
                IsCommandNavigationOrderDirty = false;
            }
            return NavigableControls;
        }

        public enum CommandFocusMode
        {
            None,
            Attached,
            Container,
        }
    }
}
