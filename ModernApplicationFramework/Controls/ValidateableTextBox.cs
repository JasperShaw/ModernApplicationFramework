using System.Globalization;
using System.Media;
using System.Windows;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Native.Platform.Enums;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Controls
{
    public abstract class ValidateableTextBox : TextBox
    {
        protected abstract bool InternalValidationRule(string s);

        protected abstract void HandleError();

        protected override void OnPreviewTextChanged(PreviewTextChangedEventArgs e)
        {
            if (e.Type == TextChangedType.Delete)
            {
                base.OnPreviewTextChanged(e);
                return;
            }
            if (InternalValidationRule(e.Text))
                base.OnPreviewTextChanged(e);
            else
            {
                e.Handled = true;
                HandleError();
            }
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

        protected override void HandleError()
        {
            new BalloonTooltip(this, CommonUI_Resources.Error_NoNumerEntered, CommonUI_Resources.Error_InvalidCharacter,
                BalloonType.Error, SystemSounds.Asterisk).Show();
        }
    }
}
