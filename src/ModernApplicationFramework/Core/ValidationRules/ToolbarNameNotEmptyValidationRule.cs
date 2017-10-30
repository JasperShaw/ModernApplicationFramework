using System.Globalization;
using System.Windows.Controls;

namespace ModernApplicationFramework.Core.ValidationRules
{
    /// <summary>
    /// A <see cref="T:System.Windows.Controls.ValidationRule" /> that checks, if a tool bar name if valid
    /// </summary>
    /// <seealso cref="System.Windows.Controls.ValidationRule" />
    public class ToolbarNameNotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult validationResult = new ValidationResult(false, "The toolbar name cannot be blank. Type a name.");
            string str = value as string;
            if (string.IsNullOrEmpty(str) || str.Trim().Length == 0)
                return validationResult;
            return ValidationResult.ValidResult;
        }
    }
}
