using System;
using System.Globalization;
using System.Windows.Controls;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal sealed class ZoomLevelValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null)
            {
                string str = value.ToString().Trim();
                string percentSymbol = cultureInfo.NumberFormat.PercentSymbol;
                if (str.Contains(percentSymbol) && str.IndexOf(percentSymbol, StringComparison.CurrentCulture) < str.Length - percentSymbol.Length)
                    return new ValidationResult(false, null);
                if (int.TryParse(str.Replace(percentSymbol, string.Empty).Trim(), out var result) && result >= 0)
                    return new ValidationResult(true, null);
            }
            return new ValidationResult(false, null);
        }
    }
}
