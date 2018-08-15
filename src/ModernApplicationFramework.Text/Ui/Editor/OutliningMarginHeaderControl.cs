using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public abstract class OutliningMarginHeaderControl : Control
    {
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.RegisterAttached(nameof(IsExpanded), typeof(bool), typeof(OutliningMarginHeaderControl));
        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.RegisterAttached(nameof(IsHighlighted), typeof(bool), typeof(OutliningMarginHeaderControl));

        static OutliningMarginHeaderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OutliningMarginHeaderControl), new FrameworkPropertyMetadata(typeof(OutliningMarginHeaderControl)));
        }

        public static void SetIsExpanded(OutliningMarginHeaderControl control, bool isExpanded)
        {
            control.SetValue(IsExpandedProperty, isExpanded);
        }

        public static bool GetIsExpanded(OutliningMarginHeaderControl control)
        {
            return true.Equals(control.GetValue(IsExpandedProperty));
        }

        public bool IsExpanded
        {
            get => GetIsExpanded(this);
            set => SetIsExpanded(this, value);
        }

        public static void SetIsHighlighted(OutliningMarginHeaderControl control, bool isExpanded)
        {
            control.SetValue(IsHighlightedProperty, isExpanded);
        }

        public static bool GetIsHighlighted(OutliningMarginHeaderControl control)
        {
            return true.Equals(control.GetValue(IsHighlightedProperty));
        }

        public bool IsHighlighted
        {
            get => GetIsHighlighted(this);
            set => SetIsHighlighted(this, value);
        }
    }
}
