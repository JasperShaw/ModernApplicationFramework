using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using ModernApplicationFramework.Basics.Definitions.Toolbar;

namespace ModernApplicationFramework.Core.ValidationRules
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="T:System.Windows.Controls.ValidationRule" /> that checks, if a given tool bar name if valid and unique
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.ValidationRule" />
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

            if (((ToolBarDataSource)toolbars.CurrentItem).Text.Equals(str, StringComparison.CurrentCultureIgnoreCase))
                return ValidationResult.ValidResult;

            foreach (ToolBarDataSource definition in toolbars)
                if (definition.Text.Equals(str, StringComparison.CurrentCultureIgnoreCase))
                    return new ValidationResult(false, $"A toolbar named '{definition.Text}' already exists. Type another name.");
            return ValidationResult.ValidResult;
        }
    }
}
