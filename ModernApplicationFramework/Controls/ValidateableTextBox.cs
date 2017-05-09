using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace ModernApplicationFramework.Controls
{
    public abstract class ValidateableTextBox : TextBox
    {
        protected abstract bool InternalValidationRule(string s);

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (InternalValidationRule(e.Text))
                base.OnPreviewTextInput(e);
            else
                e.Handled = true;
        }
    }

    public class NumberOnlyTextBox : ValidateableTextBox
    {
        public static readonly DependencyProperty NumberStyleProperty = DependencyProperty.Register(
            "NumberStyles", typeof(NumberStyles), typeof(NumberOnlyTextBox), new PropertyMetadata(default(NumberStyles)));

        public NumberStyles NumberStyle
        {
            get => (NumberStyles) GetValue(NumberStyleProperty);
            set => SetValue(NumberStyleProperty, value);
        }

        protected override bool InternalValidationRule(string s)
        {
            return int.TryParse(s, NumberStyle, CultureInfo.CurrentCulture, out int _);
        }
    }
}
