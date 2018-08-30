using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Focus
{
    public static class CommandBarNavigationHelper
    {
        public static readonly DependencyProperty CommandFocusModeProperty =
            DependencyProperty.RegisterAttached("CommandFocusMode", typeof(CommandFocusMode),
                typeof(CommandBarNavigationHelper),
                new FrameworkPropertyMetadata(CommandFocusMode.None, OnCommandFocusModePropertyChanged));

        public static readonly DependencyProperty IsCommandNavigableProperty =
            DependencyProperty.RegisterAttached("IsCommandNavigable", typeof(bool), typeof(CommandBarNavigationHelper),
                new FrameworkPropertyMetadata(false, OnIsCommandNavigablePropertyChanged));

        public static readonly DependencyProperty CommandNavigationOrderProperty =
            DependencyProperty.RegisterAttached("CommandNavigationOrder", typeof(int),
                typeof(CommandBarNavigationHelper),
                new FrameworkPropertyMetadata(Boxes.Int32Zero, OnCommandNavigationOrderPropertyChanged));

        private static readonly WeakCollection<UIElement> NavigableControls = new WeakCollection<UIElement>();

        public static event PropertyChangedCallback CommandFocusModePropertyChanged;

        private static bool IsCommandNavigationOrderDirty { get; set; }

        public enum CommandFocusMode
        {
            None,
            Attached,
            Container
        }

        public static CommandFocusMode GetCommandFocusMode(DependencyObject element)
        {
            Validate.IsNotNull(element, nameof(element));
            return (CommandFocusMode) element.GetValue(CommandFocusModeProperty);
        }

        public static int GetCommandNavigationOrder(DependencyObject element)
        {
            Validate.IsNotNull(element, nameof(element));
            return (int) element.GetValue(CommandNavigationOrderProperty);
        }

        public static bool GetIsCommandNavigable(DependencyObject element)
        {
            Validate.IsNotNull(element, nameof(element));
            return (bool) element.GetValue(IsCommandNavigableProperty);
        }

        public static void SetCommandFocusMode(DependencyObject element, CommandFocusMode value)
        {
            Validate.IsNotNull(element, nameof(element));
            element.SetValue(CommandFocusModeProperty, value);
        }

        public static void SetCommandNavigationOrder(DependencyObject element, int value)
        {
            Validate.IsNotNull(element, nameof(element));
            element.SetValue(CommandNavigationOrderProperty, value);
            IsCommandNavigationOrderDirty = true;
        }

        public static void SetIsCommandNavigable(DependencyObject element, bool value)
        {
            Validate.IsNotNull(element, nameof(element));
            element.SetValue(IsCommandNavigableProperty, value);
        }

        internal static IEnumerable<UIElement> GetSortedNavigableControls()
        {
            if (IsCommandNavigationOrderDirty)
            {
                var list = NavigableControls.OrderBy(GetCommandNavigationOrder).ToList();
                NavigableControls.Clear();
                foreach (var uiElement in list)
                    NavigableControls.Add(uiElement);
                IsCommandNavigationOrderDirty = false;
            }
            return NavigableControls;
        }

        private static void OnCommandFocusModePropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            var modePropertyChanged = CommandFocusModePropertyChanged;
            modePropertyChanged?.Invoke(obj, e);
        }

        private static void OnCommandNavigationOrderPropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            IsCommandNavigationOrderDirty = true;
        }

        private static void OnIsCommandNavigablePropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            Validate.IsNotNull(obj, nameof(obj));
            if (!(obj is UIElement control))
                return;
            if (e.NewValue is bool b && b)
                RegisterNavigableControl(control);
            else
                UnregisterNavigableControl(control);
        }

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
    }
}