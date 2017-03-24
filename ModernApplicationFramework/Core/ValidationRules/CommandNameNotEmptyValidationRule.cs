using System.Globalization;
using System.Windows.Controls;

namespace ModernApplicationFramework.Core.ValidationRules
{
    public class CommandNameNotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = new ValidationResult(false, "The command name cannot be blank. Type a name.");
            string str = value as string;
            if (string.IsNullOrEmpty(str) || str.Trim().Length == 0)
                return result;
            return ValidationResult.ValidResult;
        }
    }
}
