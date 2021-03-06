﻿using System.Globalization;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.CommandBar.Customize;

namespace ModernApplicationFramework.Core.ValidationRules
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="T:System.Windows.Controls.ValidationRule" /> that checks, if an input string can be pared to an <see langword="uint"/>
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.ValidationRule" />
    public class PositiveIntegerValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = new ValidationResult(false, Customize_Resources.Error_InvalidComboBoxWidthValue);
            var s = value as string;
            if (string.IsNullOrEmpty(s))
                return result;
            uint intResult;
            if (!uint.TryParse(s, out intResult))
                return result;
	        return ValidationResult.ValidResult;
        }
    }
}
