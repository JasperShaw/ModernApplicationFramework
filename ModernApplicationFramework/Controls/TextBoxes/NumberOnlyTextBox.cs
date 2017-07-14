using System.Globalization;
using System.Media;
using System.Windows;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Controls.TextBoxes
{
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