using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Native.Platform.Enums;

namespace ModernApplicationFramework.Controls.TextBoxes
{
    /// <inheritdoc />
    /// <summary>
    /// A special <see cref="T:ModernApplicationFramework.Controls.TextBoxes.TextBox" /> that supports input validation
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Controls.TextBoxes.TextBox" />
    public abstract class ValidateableTextBox : TextBox
    {
        /// <summary>
        /// Checks the input against a validation logic
        /// </summary>
        /// <param name="s">The to be inserted text</param>
        /// <returns></returns>
        protected abstract bool InternalValidationRule(string s);

        /// <summary>
        /// Handles an input violation.
        /// </summary>
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
}
