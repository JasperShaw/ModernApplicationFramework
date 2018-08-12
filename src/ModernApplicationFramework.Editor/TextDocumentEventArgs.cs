using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Editor
{
    public class TextDocumentEventArgs : EventArgs
    {
        public TextDocumentEventArgs(ITextDocument textDocument)
        {
            TextDocument = textDocument;
        }

        public ITextDocument TextDocument { get; }
    }
}