using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Controls.Utilities
{
    public static class FocusBehavior
    {
        public static readonly DependencyProperty FocusFirstProperty =
            DependencyProperty.RegisterAttached(
                "FocusFirst",
                typeof(bool),
                typeof(FocusBehavior),
                new PropertyMetadata(false, OnFocusFirstPropertyChanged));

        public static bool GetFocusFirst(Control control)
        {
            return (bool)control.GetValue(FocusFirstProperty);
        }

        public static void SetFocusFirst(Control control, bool value)
        {
            control.SetValue(FocusFirstProperty, value);
        }

        static void OnFocusFirstPropertyChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Control control = obj as Control;
            if (control == null || !(args.NewValue is bool))
                return;

            var uc = control.FindAncestor<UserControl>();

            if (uc == null)
                return;
            uc.Loaded += (sender, e) =>
            {
                uc.Dispatcher.BeginInvoke((Action) (() =>
                {
                    Keyboard.Focus(control);
                }), DispatcherPriority.Render);

            };
        }
    }
}
