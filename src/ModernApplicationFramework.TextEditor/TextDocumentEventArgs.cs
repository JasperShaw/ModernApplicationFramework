using System;

namespace ModernApplicationFramework.TextEditor
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