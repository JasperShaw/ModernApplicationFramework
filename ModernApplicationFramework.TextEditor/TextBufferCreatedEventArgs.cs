using System;

namespace ModernApplicationFramework.TextEditor
{
    public class TextBufferCreatedEventArgs : EventArgs
    {
        public ITextBuffer TextBuffer { get; }

        public TextBufferCreatedEventArgs(ITextBuffer textBuffer)
        {
            TextBuffer = textBuffer ?? throw new ArgumentNullException(nameof(textBuffer));
        }
    }
}