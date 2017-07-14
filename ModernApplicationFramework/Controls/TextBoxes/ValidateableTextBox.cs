using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Native.Platform.Enums;

namespace ModernApplicationFramework.Controls.TextBoxes
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
}
