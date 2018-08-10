using System;
using System.Text;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextDocumentFactoryService
    {
        ITextDocument CreateAndLoadTextDocument(string filePath, IContentType contentType);

        ITextDocument CreateAndLoadTextDocument(string filePath, IContentType contentType, Encoding encoding, out bool characterSubstitutionsOccurred);

        ITextDocument CreateAndLoadTextDocument(string filePath, IContentType contentType, bool attemptUtf8Detection, out bool characterSubstitutionsOccurred);

        ITextDocument CreateTextDocument(ITextBuffer textBuffer, string filePath);

        bool TryGetTextDocument(ITextBuffer textBuffer, out ITextDocument textDocument);

        event EventHandler<TextDocumentEventArgs> TextDocumentCreated;

        event EventHandler<TextDocumentEventArgs> TextDocumentDisposed;
    }
}