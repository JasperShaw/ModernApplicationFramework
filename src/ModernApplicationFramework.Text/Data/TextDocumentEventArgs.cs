using System;

namespace ModernApplicationFramework.Text.Data
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