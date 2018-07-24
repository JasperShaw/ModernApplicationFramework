using System;

namespace ModernApplicationFramework.TextEditor
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