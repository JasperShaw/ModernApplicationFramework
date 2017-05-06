using System.Globalization;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.CustomizeDialog;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Core.ValidationRules
{
    public class CommandNameNotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = new ValidationResult(false, Customize_Resources.Error_RenameCommandEmptyName);
            string str = value as string;
            if (string.IsNullOrEmpty(str) || str.Trim().Length == 0)
                return result;
            return ValidationResult.ValidResult;
        }
    }
}
