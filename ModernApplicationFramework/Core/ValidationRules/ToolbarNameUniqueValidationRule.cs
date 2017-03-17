using System.Globalization;
using System.Windows.Controls;

namespace ModernApplicationFramework.Core.ValidationRules
{
    public class ToolbarNameUniqueValidationRule : ValidationRule
    {
        public ToolbarNameUniqueValidationRuleDataContext DataContext { get; set; }


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return ValidationResult.ValidResult;
        }
    }
}
