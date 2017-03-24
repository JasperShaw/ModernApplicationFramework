using System.Globalization;
using System.Windows.Controls;

namespace ModernApplicationFramework.Core.ValidationRules
{
    public class ComboBoxWidthValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = new ValidationResult(false, "The width must be a positive integer value or 0 to use default width.");
            var s = value as string;
            if (string.IsNullOrEmpty(s))
                return result;
            uint intResult = 0;
            if (!uint.TryParse(s, out intResult))
                return result;
            return ValidationResult.ValidResult;
        }
    }
}
