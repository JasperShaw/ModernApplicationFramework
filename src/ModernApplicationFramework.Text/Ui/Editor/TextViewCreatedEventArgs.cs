using System;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public class TextViewCreatedEventArgs : EventArgs
    {
        public ITextView TextView { get; }

        public TextViewCreatedEventArgs(ITextView textView)
        {
            TextView = textView ?? throw new ArgumentNullException(nameof(textView));
        }
    }
}