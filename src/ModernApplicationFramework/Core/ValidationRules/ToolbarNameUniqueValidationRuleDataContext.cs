using System.Windows;

namespace ModernApplicationFramework.Core.ValidationRules
{
    public class ToolbarNameUniqueValidationRuleDataContext : FrameworkElement
    {
        public static readonly DependencyProperty ToolbarsProperty;

        public object Toolbars
        {
            get => GetValue(ToolbarsProperty);
            set => SetValue(ToolbarsProperty, value);
        }

        static ToolbarNameUniqueValidationRuleDataContext()
        {
            ToolbarsProperty = DependencyProperty.Register("Toolbars", typeof(object), typeof(ToolbarNameUniqueValidationRuleDataContext));
        }

    }
}
