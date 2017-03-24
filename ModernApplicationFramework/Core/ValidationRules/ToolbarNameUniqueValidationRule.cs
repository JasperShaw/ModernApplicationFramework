using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using ModernApplicationFramework.Basics.Definitions.Toolbar;

namespace ModernApplicationFramework.Core.ValidationRules
{
    public class ToolbarNameUniqueValidationRule : ValidationRule
    {
        public ToolbarNameUniqueValidationRuleDataContext DataContext { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var s = (string)value;
            if (s == null)
                return ValidationResult.ValidResult;
            var str = s.Trim();
            var toolbars = (CollectionView)DataContext.Toolbars;

            if (((ToolbarDefinition)toolbars.CurrentItem).Name.Equals(str, StringComparison.CurrentCultureIgnoreCase))
                return ValidationResult.ValidResult;

            foreach (ToolbarDefinition definition in toolbars)
                if (definition.Name.Equals(str, StringComparison.CurrentCultureIgnoreCase))
                    return new ValidationResult(false, $"A toolbar named '{definition.Name}' already exists. Type another name.");
            return ValidationResult.ValidResult;
        }
    }
}
