using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Core.ValidationRules
{
    public class RangeValidationRule : BindableValidationRule
    {
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(int), typeof(RangeValidationRule));
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(RangeValidationRule));

        public int Minimum
        {
            get => (int)BindingTarget.GetValue(MinimumProperty);
            set => BindingTarget.SetValue(MinimumProperty, value);
        }

        public int Maximum
        {
            get => (int)BindingTarget.GetValue(MaximumProperty);
            set => BindingTarget.SetValue(MaximumProperty, value);
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var validationResult = new ValidationResult(false, string.Format(CultureInfo.CurrentCulture, CommonUI_Resources.Error_IntegerValueNotInRange, new object[]
            {
                Minimum,
                Maximum
            }));
            var s = value as string;
            if (string.IsNullOrEmpty(s) || !int.TryParse(s, out var result) || result < Minimum || result > Maximum)
                return validationResult;
            return ValidationResult.ValidResult;
        }
    }
}
