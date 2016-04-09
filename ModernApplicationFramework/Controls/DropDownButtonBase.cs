using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Controls
{
    internal class DropDownButtonBase : ContentControl
    {
        public static readonly DependencyProperty RenderCheckedProperty = DependencyProperty.Register("RenderChecked",
            typeof(bool), typeof(DropDownButtonBase), new UIPropertyMetadata(false, OnRenderCheckedChanged));

        public static readonly DependencyProperty RenderEnabledProperty = DependencyProperty.Register("RenderEnabled",
            typeof(bool), typeof(DropDownButtonBase), new UIPropertyMetadata(true, OnRenderEnabledChanged));

        public static readonly DependencyProperty RenderFocusedProperty = DependencyProperty.Register("RenderFocused",
            typeof(bool), typeof(DropDownButtonBase), new UIPropertyMetadata(false, OnRenderFocusedChanged));

        public static readonly DependencyProperty RenderMouseOverProperty =
            DependencyProperty.Register("RenderMouseOver", typeof(bool), typeof(DropDownButtonBase),
                new UIPropertyMetadata(false, OnRenderMouseOverChanged));

        public static readonly DependencyProperty RenderNormalProperty = DependencyProperty.Register("RenderNormal",
            typeof(bool), typeof(DropDownButtonBase), new UIPropertyMetadata(true, OnRenderNormalChanged));

        public static readonly DependencyProperty RenderPressedProperty = DependencyProperty.Register("RenderPressed",
            typeof(bool), typeof(DropDownButtonBase), new UIPropertyMetadata(false, OnRenderPressedChanged));

        #region Constructors

        static DropDownButtonBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButtonBase),
                new FrameworkPropertyMetadata(typeof(DropDownButtonBase)));
        }

        #endregion

        #region RenderChecked

        public bool RenderChecked
        {
            get { return (bool) GetValue(RenderCheckedProperty); }
            set { SetValue(RenderCheckedProperty, value); }
        }

        private static void OnRenderCheckedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var buttonChrome = o as DropDownButtonBase;
            buttonChrome?.OnRenderCheckedChanged((bool) e.OldValue, (bool) e.NewValue);
        }

        protected virtual void OnRenderCheckedChanged(bool oldValue, bool newValue) {}

        #endregion

        #region RenderEnabled

        public bool RenderEnabled
        {
            get { return (bool) GetValue(RenderEnabledProperty); }
            set { SetValue(RenderEnabledProperty, value); }
        }

        private static void OnRenderEnabledChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var buttonChrome = o as DropDownButtonBase;
            buttonChrome?.OnRenderEnabledChanged((bool) e.OldValue, (bool) e.NewValue);
        }

        protected virtual void OnRenderEnabledChanged(bool oldValue, bool newValue) {}

        #endregion

        #region RenderFocused

        public bool RenderFocused
        {
            get { return (bool) GetValue(RenderFocusedProperty); }
            set { SetValue(RenderFocusedProperty, value); }
        }

        private static void OnRenderFocusedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var buttonChrome = o as DropDownButtonBase;
            buttonChrome?.OnRenderFocusedChanged((bool) e.OldValue, (bool) e.NewValue);
        }

        protected virtual void OnRenderFocusedChanged(bool oldValue, bool newValue) {}

        #endregion

        #region RenderMouseOver

        public bool RenderMouseOver
        {
            get { return (bool) GetValue(RenderMouseOverProperty); }
            set { SetValue(RenderMouseOverProperty, value); }
        }

        private static void OnRenderMouseOverChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var buttonChrome = o as DropDownButtonBase;
            buttonChrome?.OnRenderMouseOverChanged((bool) e.OldValue, (bool) e.NewValue);
        }

        protected virtual void OnRenderMouseOverChanged(bool oldValue, bool newValue) {}

        #endregion

        #region RenderNormal

        public bool RenderNormal
        {
            get { return (bool) GetValue(RenderNormalProperty); }
            set { SetValue(RenderNormalProperty, value); }
        }

        private static void OnRenderNormalChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var buttonChrome = o as DropDownButtonBase;
            buttonChrome?.OnRenderNormalChanged((bool) e.OldValue, (bool) e.NewValue);
        }

        protected virtual void OnRenderNormalChanged(bool oldValue, bool newValue) {}

        #endregion

        #region RenderPressed

        public bool RenderPressed
        {
            get { return (bool) GetValue(RenderPressedProperty); }
            set { SetValue(RenderPressedProperty, value); }
        }

        private static void OnRenderPressedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var buttonChrome = o as DropDownButtonBase;
            buttonChrome?.OnRenderPressedChanged((bool) e.OldValue, (bool) e.NewValue);
        }

        protected virtual void OnRenderPressedChanged(bool oldValue, bool newValue) {}

        #endregion //RenderPressed
    }
}