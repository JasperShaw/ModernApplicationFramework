using System;

namespace ModernApplicationFramework.Text.Data
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