using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Core.ValidationRules
{
    public abstract class BindableValidationRule : ValidationRule
    {
        public DependencyObject BindingTarget { get; } = new DependencyObject();
    }
}
