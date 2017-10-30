using System.Globalization;
using System.Media;
using System.Windows;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Controls.TextBoxes
{
    /// <inheritdoc />
    /// <summary>
    /// A custom <see cref="T:ModernApplicationFramework.Controls.TextBoxes.ValidateableTextBox" /> that only accepts numbers and shows a <see cref="T:ModernApplicationFramework.Controls.BalloonTooltip" /> on error
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Controls.TextBoxes.ValidateableTextBox" />
    public class NumberOnlyTextBox : ValidateableTextBox
    {
        public static readonly DependencyProperty NumberStyleProperty = DependencyProperty.Register(
            "NumberStyles", typeof(NumberStyles), typeof(NumberOnlyTextBox), new PropertyMetadata(default(NumberStyles)));

        /// <summary>
        /// The accepted number style
        /// </summary>
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