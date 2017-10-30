using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.Extensions
{
    /// <summary>
    /// Extension to focus an element when its control was loaded
    /// </summary>
    public static class FocusBehavior
    {
        /// <summary>
        /// The element to focus
        /// </summary>
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

        /// <summary>
        /// Enables the focus on this element.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
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
