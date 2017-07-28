namespace ModernApplicationFramework.Controls.TextBoxes
{
    /// <inheritdoc />
    /// <summary>
    /// A custom <see cref="T:ModernApplicationFramework.Controls.TextBoxes.ValidateableTextBox" /> that only accepts numbers and shows a <see cref="T:ModernApplicationFramework.Controls.BalloonTooltip" /> on error
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Controls.TextBoxes.ValidateableTextBox" />
    public class KeyGestureTextBox : ValidateableTextBox
    {
        protected override bool InternalValidationRule(string s)
        {
            return true;
        }

        protected override void HandleError()
        {

        }
    }
}