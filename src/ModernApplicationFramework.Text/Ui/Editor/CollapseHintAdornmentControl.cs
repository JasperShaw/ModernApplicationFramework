using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public abstract class CollapseHintAdornmentControl : Control
    {
        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.RegisterAttached(nameof(IsHighlighted), typeof(bool), typeof(CollapseHintAdornmentControl));
        public static readonly DependencyProperty IsHighContrastProperty = DependencyProperty.RegisterAttached(nameof(IsHighContrast), typeof(bool), typeof(CollapseHintAdornmentControl));

        public static void SetIsHighlighted(CollapseHintAdornmentControl control, bool isExpanded)
        {
            control.SetValue(IsHighlightedProperty, isExpanded);
        }

        public static bool GetIsHighlighted(CollapseHintAdornmentControl control)
        {
            return true.Equals(control.GetValue(IsHighlightedProperty));
        }

        public bool IsHighlighted
        {
            get => GetIsHighlighted(this);
            set => SetIsHighlighted(this, value);
        }

        public static bool GetIsHighContrast(CollapseHintAdornmentControl control)
        {
            return true.Equals(control.GetValue(IsHighContrastProperty));
        }

        public bool IsHighContrast => GetIsHighContrast(this);

        protected CollapseHintAdornmentControl()
        {
            SetResourceReference(IsHighContrastProperty, SystemParameters.HighContrastKey);
        }

        static CollapseHintAdornmentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CollapseHintAdornmentControl), new FrameworkPropertyMetadata(typeof(CollapseHintAdornmentControl)));
        }
    }
}
