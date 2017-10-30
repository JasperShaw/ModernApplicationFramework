using System.Globalization;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.CustomizeDialog;

namespace ModernApplicationFramework.Core.ValidationRules
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="T:System.Windows.Controls.ValidationRule" /> that checks, if an command name is valid
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.ValidationRule" />
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
