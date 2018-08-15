using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public abstract class OutliningMarginBracketControl : Control
    {
        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.RegisterAttached(nameof(IsHighlighted), typeof(bool), typeof(OutliningMarginBracketControl), new FrameworkPropertyMetadata(IsHighlightedPropertyChangedCallback));
        public static readonly DependencyProperty FirstLineOffsetProperty = DependencyProperty.RegisterAttached(nameof(FirstLineOffset), typeof(double), typeof(OutliningMarginBracketControl));

        static OutliningMarginBracketControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OutliningMarginBracketControl), new FrameworkPropertyMetadata(typeof(OutliningMarginBracketControl)));
        }

        public static void SetIsHighlighted(OutliningMarginBracketControl control, bool isExpanded)
        {
            control.SetValue(IsHighlightedProperty, isExpanded);
        }

        public static bool GetIsHighlighted(OutliningMarginBracketControl control)
        {
            return true.Equals(control.GetValue(IsHighlightedProperty));
        }

        public bool IsHighlighted
        {
            get => GetIsHighlighted(this);
            set => SetIsHighlighted(this, value);
        }

        private static void IsHighlightedPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((OutliningMarginBracketControl)d).OnIsHighlightedChanged((bool)args.NewValue);
        }

        protected virtual void OnIsHighlightedChanged(bool newValue)
        {
        }

        public static void SetFirstLineOffset(OutliningMarginBracketControl control, double firstLineOffset)
        {
            control.SetValue(FirstLineOffsetProperty, firstLineOffset);
        }

        public static double GetFirstLineOffset(OutliningMarginBracketControl control)
        {
            return (double)control.GetValue(FirstLineOffsetProperty);
        }

        public double FirstLineOffset
        {
            get => GetFirstLineOffset(this);
            set => SetFirstLineOffset(this, value);
        }
    }
}
